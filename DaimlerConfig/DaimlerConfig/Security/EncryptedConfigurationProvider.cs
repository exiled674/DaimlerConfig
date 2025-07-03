using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace DConfig.Security
{
    public class EncryptedConfigurationProvider : FileConfigurationProvider
    {
        private readonly bool _developmentMode;

        // Öffentliche Property für Entschlüsselungsfehler
        public bool HasDecryptionErrors { get; private set; }
        public List<string> DecryptionErrors { get; private set; } = new List<string>();

        public EncryptedConfigurationProvider(FileConfigurationSource source, bool developmentMode = false)
            : base(source)
        {
            _developmentMode = developmentMode;
        }

        public override void Load(Stream stream)
        {
            // Reset error state
            HasDecryptionErrors = false;
            DecryptionErrors.Clear();

            var parsedData = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (stream == null || stream.Length == 0)
            {
                Data = parsedData;
                return;
            }

            try
            {
                using (JsonDocument document = JsonDocument.Parse(stream))
                {
                    if (document.RootElement.ValueKind != JsonValueKind.Object)
                    {
                        throw new FormatException("Top-level JSON element must be an object.");
                    }

                    LoadJsonElement(document.RootElement, parsedData, null);
                }
            }
            catch (JsonException ex)
            {
                throw new FormatException($"Error parsing JSON file: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error loading configuration from file: {ex.Message}", ex);
            }

            Data = parsedData;

            // Apply decryption if not in development mode
            if (!_developmentMode)
            {
                DecryptConnectionStrings();
            }
        }

        private void LoadJsonElement(JsonElement element, IDictionary<string, string> data, string prefix)
        {
            foreach (var property in element.EnumerateObject())
            {
                string key = string.IsNullOrEmpty(prefix) ? property.Name : $"{prefix}:{property.Name}";

                switch (property.Value.ValueKind)
                {
                    case JsonValueKind.Object:
                        LoadJsonElement(property.Value, data, key);
                        break;
                    case JsonValueKind.Array:
                        int index = 0;
                        foreach (var item in property.Value.EnumerateArray())
                        {
                            LoadJsonElement(item, data, $"{key}:{index++}");
                        }
                        break;
                    case JsonValueKind.String:
                    case JsonValueKind.Number:
                    case JsonValueKind.True:
                    case JsonValueKind.False:
                    case JsonValueKind.Null:
                        data[key] = property.Value.ToString();
                        break;
                }
            }
        }

        private void DecryptConnectionStrings()
        {
            var keysToDecrypt = new List<string>();

            foreach (var kvp in Data)
            {
                if (kvp.Key.StartsWith("ConnectionStrings:") &&
                    ConfigurationEncryptor.IsEncrypted(kvp.Value))
                {
                    keysToDecrypt.Add(kvp.Key);
                }
            }

            // Entschlüsselung mit Fehlerbehandlung - App stürzt NICHT ab!
            foreach (var key in keysToDecrypt)
            {
                try
                {
                    Data[key] = ConfigurationEncryptor.DecryptConnectionString(Data[key]);
                }
                catch (Exception ex)
                {
                    // Fehler sammeln statt Exception zu werfen
                    HasDecryptionErrors = true;
                    var errorMessage = $"Fehler beim Entschlüsseln der Konfiguration für '{key}': {ex.Message}";
                    DecryptionErrors.Add(errorMessage);

                    // ConnectionString als ungültig markieren (leerer String)
                    Data[key] = string.Empty;

                    // Optional: Logging falls verfügbar
                    System.Diagnostics.Debug.WriteLine($"[EncryptedConfigurationProvider] {errorMessage}");
                }
            }
        }
    }

    public class EncryptedConfigurationSource : FileConfigurationSource
    {
        public bool DevelopmentMode { get; set; }

        public override IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            EnsureDefaults(builder);
            return new EncryptedConfigurationProvider(this, DevelopmentMode);
        }
    }
}
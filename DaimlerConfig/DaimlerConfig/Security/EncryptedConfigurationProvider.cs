using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json; // NEW: Add this using directive for System.Text.Json

namespace DaimlerConfig.Security
{
    public class EncryptedConfigurationProvider : FileConfigurationProvider
    {
        private readonly bool _developmentMode;

        public EncryptedConfigurationProvider(FileConfigurationSource source, bool developmentMode = false)
            : base(source)
        {
            _developmentMode = developmentMode;
        }

        // This is the abstract method from FileConfigurationProvider that must be implemented.
        // It's responsible for parsing the stream into key-value pairs for the Data dictionary.
        public override void Load(Stream stream)
        {
            // 1. Parse the JSON from the stream into a flat dictionary
            // We implement this logic ourselves using System.Text.Json.
            var parsedData = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (stream == null || stream.Length == 0)
            {
                // Handle empty or missing file gracefully, as base.Load() might do for optional files
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

                    // Recursively load the JSON elements into the flat dictionary
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

            // Assign the parsed data to the provider's Data property
            Data = parsedData;

            // 2. Apply decryption if not in development mode
            if (!_developmentMode)
            {
                DecryptConnectionStrings();
            }
        }

        // Helper method to recursively load JSON elements into a flat dictionary
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
                            // For arrays, keys are typically "ParentKey:Index:ChildKey"
                            LoadJsonElement(item, data, $"{key}:{index++}");
                        }
                        break;
                    case JsonValueKind.String:
                    case JsonValueKind.Number:
                    case JsonValueKind.True:
                    case JsonValueKind.False:
                    case JsonValueKind.Null:
                        // Convert all simple values to string and store
                        data[key] = property.Value.ToString();
                        break;
                }
            }
        }

        private void DecryptConnectionStrings()
        {
            var keysToDecrypt = new List<string>();

            // Find all Connection String Keys that are encrypted
            // We iterate over a copy of keys to avoid modifying the collection during enumeration
            foreach (var kvp in Data)
            {
                if (kvp.Key.StartsWith("ConnectionStrings:") &&
                    ConfigurationEncryptor.IsEncrypted(kvp.Value))
                {
                    keysToDecrypt.Add(kvp.Key);
                }
            }

            // Decrypt them and update the Data dictionary
            foreach (var key in keysToDecrypt)
            {
                try
                {
                    Data[key] = ConfigurationEncryptor.DecryptConnectionString(Data[key]);
                }
                catch (Exception ex)
                {
                    // Propagate the exception as a configuration error
                    throw new InvalidOperationException($"Fehler beim Entschlüsseln der Konfiguration für '{key}': {ex.Message}", ex);
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
            // Return our custom provider that uses the source settings
            return new EncryptedConfigurationProvider(this, DevelopmentMode);
        }
    }
}
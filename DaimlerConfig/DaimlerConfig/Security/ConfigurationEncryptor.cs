using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace DaimlerConfig.Security
{
    public static class ConfigurationEncryptor
    {
        private const string ENCRYPTION_PREFIX = "ENC:";
        private const string KEY_FILE_NAME = ".appkey";

        /// <summary>
        /// Verschlüsselt einen Connection String und speichert ihn in appsettings.json
        /// </summary>
        public static void EncryptConnectionString(string plainConnectionString, string appSettingsPath)
        {
            var key = GetOrCreateEncryptionKey();
            var encryptedValue = EncryptString(plainConnectionString, key);

            // appsettings.json lesen und aktualisieren
            string jsonContent = File.ReadAllText(appSettingsPath);
            var jsonDoc = JsonDocument.Parse(jsonContent);

            var options = new JsonWriterOptions { Indented = true };
            using var stream = new MemoryStream();
            using var writer = new Utf8JsonWriter(stream, options);

            writer.WriteStartObject();

            foreach (var property in jsonDoc.RootElement.EnumerateObject())
            {
                if (property.Name == "ConnectionStrings")
                {
                    writer.WriteStartObject("ConnectionStrings");
                    foreach (var connString in property.Value.EnumerateObject())
                    {
                        if (connString.Name == "DefaultConnection")
                        {
                            writer.WriteString("DefaultConnection", ENCRYPTION_PREFIX + encryptedValue);
                        }
                        else
                        {
                            writer.WriteString(connString.Name, connString.Value.GetString());
                        }
                    }
                    writer.WriteEndObject();
                }
                else
                {
                    property.WriteTo(writer);
                }
            }

            writer.WriteEndObject();

            File.WriteAllText(appSettingsPath, Encoding.UTF8.GetString(stream.ToArray()));
        }

        /// <summary>
        /// Entschlüsselt einen Connection String zur Laufzeit
        /// </summary>
        public static string DecryptConnectionString(string encryptedValue)
        {
            if (!encryptedValue.StartsWith(ENCRYPTION_PREFIX))
                return encryptedValue; // Nicht verschlüsselt

            var actualEncryptedValue = encryptedValue.Substring(ENCRYPTION_PREFIX.Length);
            var key = GetOrCreateEncryptionKey();
            return DecryptString(actualEncryptedValue, key);
        }

        /// <summary>
        /// Prüft ob ein Wert verschlüsselt ist
        /// </summary>
        public static bool IsEncrypted(string value)
        {
            return value?.StartsWith(ENCRYPTION_PREFIX) == true;
        }

        private static byte[] GetOrCreateEncryptionKey()
        {
            string userFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string keyPath = Path.Combine(userFolder, KEY_FILE_NAME);

            if (File.Exists(keyPath))
            {
                return File.ReadAllBytes(keyPath);
            }

            // Neuen Schlüssel generieren
            using var rng = RandomNumberGenerator.Create();
            var key = new byte[32]; // 256-bit key für AES
            rng.GetBytes(key);

            File.WriteAllBytes(keyPath, key);

            // Datei verstecken (Windows)
            try
            {
                File.SetAttributes(keyPath, FileAttributes.Hidden);
            }
            catch { /* Ignorieren falls nicht möglich */ }

            return key;
        }

        private static string EncryptString(string plainText, byte[] key)
        {
            using var aes = Aes.Create();
            aes.Key = key;
            aes.GenerateIV();

            using var encryptor = aes.CreateEncryptor();
            using var msEncrypt = new MemoryStream();

            // IV an den Anfang schreiben
            msEncrypt.Write(aes.IV, 0, aes.IV.Length);

            using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
            using (var swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(plainText);
            }

            return Convert.ToBase64String(msEncrypt.ToArray());
        }

        private static string DecryptString(string encryptedText, byte[] key)
        {
            var fullCipher = Convert.FromBase64String(encryptedText);

            using var aes = Aes.Create();
            aes.Key = key;

            // IV aus den ersten 16 Bytes extrahieren
            var iv = new byte[16];
            Array.Copy(fullCipher, 0, iv, 0, iv.Length);
            aes.IV = iv;

            using var decryptor = aes.CreateDecryptor();
            using var msDecrypt = new MemoryStream(fullCipher, iv.Length, fullCipher.Length - iv.Length);
            using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using var srDecrypt = new StreamReader(csDecrypt);

            return srDecrypt.ReadToEnd();
        }
    }
}
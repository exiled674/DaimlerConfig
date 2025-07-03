using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DConfig.Security
{
    /// <summary>
    /// Console Utility zum Verschlüsseln von Connection Strings
    /// </summary>
    public class EncryptionUtility
    {
        /// <summary>
        /// Verschlüsselt einen Connection String in der appsettings.json
        /// </summary>
        /// <param name="connectionString">Der zu verschlüsselnde Connection String</param>
        /// <param name="appSettingsPath">Pfad zur appsettings.json Datei</param>
        public static void EncryptConnectionString(string connectionString, string appSettingsPath)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException("Connection String darf nicht leer sein.");
            }

            if (!File.Exists(appSettingsPath))
            {
                throw new FileNotFoundException($"appsettings.json nicht gefunden: {appSettingsPath}");
            }

            try
            {
                ConfigurationEncryptor.EncryptConnectionString(connectionString, appSettingsPath);
                Console.WriteLine("✓ Connection String erfolgreich verschlüsselt!");
                Console.WriteLine($"✓ appsettings.json aktualisiert: {appSettingsPath}");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Fehler beim Verschlüsseln: {ex.Message}", ex);
            }
        }


    }
}
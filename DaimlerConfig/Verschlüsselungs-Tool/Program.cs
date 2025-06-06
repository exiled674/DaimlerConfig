using System;
using System.IO;
using ConnectionStringEncryptor; // Stellt sicher, dass dies Ihr Projekt-Namespace ist

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Connection String Encryption Utility ===");

        string appSettingsPath = string.Empty;
        string connectionString = string.Empty;

        if (args.Length == 0)
        {
            // Interaktiver Modus
            Console.WriteLine("\nInteraktiver Modus:");
            Console.Write("Bitte geben Sie den zu verschlüsselnden Connection String ein: ");
            connectionString = Console.ReadLine();

            // Standardmäßig im aktuellen Verzeichnis nach appsettings.json suchen
            string defaultAppSettingsPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");

            Console.WriteLine($"\nStandard-Pfad für appsettings.json (im Tool-Verzeichnis): {defaultAppSettingsPath}");
            Console.Write("Möchten Sie diesen Pfad verwenden? (J/N, Standard: J): ");
            var useDefaultPath = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(useDefaultPath) || useDefaultPath.Trim().ToUpper() == "J")
            {
                appSettingsPath = defaultAppSettingsPath;
            }
            else
            {
                Console.Write("Bitte geben Sie den vollständigen Pfad zur appsettings.json Datei ein: ");
                appSettingsPath = Console.ReadLine();
            }
        }
        else if (args.Length == 2)
        {
            // Kommandozeilen-Modus
            connectionString = args[0];
            appSettingsPath = args[1];
        }
        else
        {
            Console.WriteLine("\nFehler: Ungültige Anzahl von Argumenten.");
            Console.WriteLine("Verwendung (interaktiver Modus): ConnectionStringEncryptor.exe");
            Console.WriteLine("Verwendung (Kommandozeilen-Modus): ConnectionStringEncryptor.exe \"<connection-string>\" \"<appsettings-path>\"");
            Console.WriteLine("Beispiel (Tool-Verzeichnis): ConnectionStringEncryptor.exe \"Server=...;Database=...\" \".\\appsettings.json\"");
            Console.WriteLine("Beispiel (voller Pfad): ConnectionStringEncryptor.exe \"Server=...;Database=...\" \"C:\\Users\\IhrUsername\\appsettings.json\"");
            Console.WriteLine("\nDrücken Sie eine beliebige Taste zum Beenden.");
            Console.ReadKey();
            return;
        }

        EncryptAndSave(connectionString, appSettingsPath);

        Console.WriteLine("\nDrücken Sie eine beliebige Taste zum Beenden.");
        Console.ReadKey();
    }

    private static void EncryptAndSave(string connectionString, string appSettingsPath)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                Console.WriteLine("✗ Fehler: Connection String darf nicht leer sein.");
                return;
            }

            if (string.IsNullOrWhiteSpace(appSettingsPath))
            {
                Console.WriteLine("✗ Fehler: Pfad zur appsettings.json darf nicht leer sein.");
                return;
            }

            // Prüfen, ob der Pfad ein relativer Pfad ist (z.B. ".\appsettings.json")
            // und ihn in einen absoluten Pfad umwandeln, relativ zum aktuellen Verzeichnis
            if (!Path.IsPathRooted(appSettingsPath))
            {
                appSettingsPath = Path.Combine(Environment.CurrentDirectory, appSettingsPath);
            }

            if (!File.Exists(appSettingsPath))
            {
                Console.WriteLine($"✗ Fehler: appsettings.json nicht gefunden unter: {appSettingsPath}");
                Console.WriteLine("Stellen Sie sicher, dass die appsettings.json-Datei existiert und der Pfad korrekt ist.");
                return;
            }

            // Call the encryption logic from ConfigurationEncryptor
            ConfigurationEncryptor.EncryptConnectionString(connectionString, appSettingsPath);

            Console.WriteLine("\n✓ Connection String erfolgreich verschlüsselt!");
            Console.WriteLine($"✓ appsettings.json aktualisiert: {appSettingsPath}");
            Console.WriteLine($"✓ Ein Schlüssel (.appkey) wurde generiert oder verwendet in: {Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".appkey")}");
            Console.WriteLine("\nBITTE BEACHTEN SIE: Der Schlüssel (.appkey) ist versteckt.");
            Console.WriteLine("Sichern Sie Ihre appsettings.json (Backup der unverschlüsselten) und die .appkey Datei sorgfältig!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n✗ Ein unerwarteter Fehler ist aufgetreten: {ex.Message}");
            Console.WriteLine($"Details: {ex.ToString()}");
        }
    }
}
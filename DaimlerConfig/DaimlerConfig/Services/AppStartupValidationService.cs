using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.SignalR.Client;
using System.IO;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using DaimlerConfig.Security;

namespace DaimlerConfig.Services
{
    public class AppStartupValidationService
    {
        private readonly IConfiguration _configuration;
        private readonly IConfigurationRoot _configurationRoot;
        private readonly string _appSettingsPath;

        public string ErrorMessage { get; private set; } = string.Empty;
        public bool HasErrors { get; private set; } = false;

        public AppStartupValidationService(IConfiguration configuration)
        {
            _configuration = configuration;
            _configurationRoot = configuration as IConfigurationRoot;
            _appSettingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "appsettings.json");

            // Synchrone Validierung beim Start
            ValidateAppStartup();
        }

        private void ValidateAppStartup()
        {
            var errors = new List<string>();

            // 1. Prüfung: appsettings.json existiert
            if (!File.Exists(_appSettingsPath))
            {
                errors.Add("Die Konfigurationsdatei 'appsettings.json' wurde nicht gefunden.");
            }

            // 2. Prüfung: Entschlüsselungsfehler in der Konfiguration
            if (_configurationRoot != null)
            {
                foreach (var provider in _configurationRoot.Providers)
                {
                    if (provider is EncryptedConfigurationProvider encryptedProvider &&
                        encryptedProvider.HasDecryptionErrors)
                    {
                        errors.AddRange(encryptedProvider.DecryptionErrors);
                    }
                }
            }

            // 3. Prüfung: ConnectionString ist gültig
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                errors.Add("Der Datenbankverbindungsstring ist ungültig oder fehlt.");
            }
            else
            {
                // Zusätzliche Validierung des ConnectionStrings
                try
                {
                    var builder = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(connectionString);
                    if (string.IsNullOrWhiteSpace(builder.DataSource))
                    {
                        errors.Add("Der Datenbankverbindungsstring enthält keine gültige Datenquelle.");
                    }
                }
                catch (Exception)
                {
                    errors.Add("Der Datenbankverbindungsstring hat ein ungültiges Format.");
                }
            }

            // 4. Prüfung: SignalR-Server ist erreichbar
            var hubUrl = _configuration["SignalR:HubUrl"];
            if (!string.IsNullOrWhiteSpace(hubUrl))
            {
                if (!IsSignalRServerReachable(hubUrl))
                {
                    errors.Add($"Der SignalR-Server ist nicht erreichbar: {hubUrl}");
                }
            }

            // Fehler zusammenfassen
            if (errors.Any())
            {
                HasErrors = true;
                ErrorMessage = string.Join("\n• ", new[] { "Folgende Probleme wurden festgestellt:" }.Concat(errors));
            }
        }

        private bool IsSignalRServerReachable(string hubUrl)
        {
            try
            {
                var uri = new Uri(hubUrl);
                var host = uri.Host;
                var port = uri.Port;

                // Einfacher TCP-Verbindungstest
                using var tcpClient = new TcpClient();
                var connectTask = tcpClient.ConnectAsync(host, port);
                var completed = connectTask.Wait(TimeSpan.FromSeconds(3)); // 3 Sekunden Timeout

                return completed && tcpClient.Connected;
            }
            catch
            {
                return false;
            }
        }

        public void RefreshValidation()
        {
            HasErrors = false;
            ErrorMessage = string.Empty;
            ValidateAppStartup();
        }
    }
}
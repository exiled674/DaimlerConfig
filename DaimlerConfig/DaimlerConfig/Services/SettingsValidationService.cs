using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Data.SqlClient;
using System.Net.Sockets;


namespace DaimlerConfig.Services
{
    public class SettingsValidationService
    {
        public class ValidationResult
        {
            public bool IsValid { get; set; }
            public List<string> ErrorMessages { get; set; } = new List<string>();
        }

        /// <summary>
        /// Validates both database connection and SignalR server connectivity
        /// </summary>
        /// <param name="connectionString">Database connection string to test</param>
        /// <param name="signalRUrl">SignalR server URL to test</param>
        /// <returns>ValidationResult containing success status and error messages</returns>
        public async Task<ValidationResult> ValidateSettingsAsync(string connectionString, string signalRUrl)
        {
            var result = new ValidationResult { IsValid = true };

            // Test database connection
            var dbResult = await TestDatabaseConnectionAsync(connectionString);
            if (!dbResult)
            {
                result.IsValid = false;
                result.ErrorMessages.Add("The new database settings are invalid.");
            }

            // Test SignalR server connection
            var signalRResult = await TestSignalRConnectionAsync(signalRUrl);
            if (!signalRResult)
            {
                result.IsValid = false;
                result.ErrorMessages.Add("Cannot connect to the SignalR server using the new URL.");
            }

            return result;
        }

        /// <summary>
        /// Tests database connection by attempting to open a connection
        /// </summary>
        /// <param name="connectionString">Connection string to test</param>
        /// <returns>True if connection successful, false otherwise</returns>
        private async Task<bool> TestDatabaseConnectionAsync(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                return false;

            try
            {
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                // Simple test query to ensure the connection works
                using var command = new SqlCommand("SELECT 1", connection);
                var result = await command.ExecuteScalarAsync();

                return result != null;
            }
            catch (SqlException sqlEx)
            {
                System.Diagnostics.Debug.WriteLine($"[SettingsValidation] SQL Error: {sqlEx.Message}");
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SettingsValidation] Connection Error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Tests SignalR server connectivity by attempting a TCP connection
        /// </summary>
        /// <param name="signalRUrl">SignalR hub URL to test</param>
        /// <returns>True if TCP connection successful, false otherwise</returns>
        private async Task<bool> TestSignalRConnectionAsync(string signalRUrl)
        {
            if (string.IsNullOrWhiteSpace(signalRUrl))
                return false;

            try
            {
                var uri = new Uri(signalRUrl);
                var host = uri.Host;
                var port = uri.Port;

                // Default ports if not specified
                if (port == -1)
                {
                    port = uri.Scheme.ToLower() == "https" ? 443 : 80;
                }

                using var tcpClient = new TcpClient();

                // Use CancellationToken for 2-second timeout
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));

                try
                {
                    await tcpClient.ConnectAsync(host, port, cts.Token);
                    return tcpClient.Connected;
                }
                catch (OperationCanceledException)
                {
                    // Timeout occurred
                    return false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SettingsValidation] SignalR Connection Error: {ex.Message}");
                return false;
            }
        }
    }
}

using Xunit;
using Dapper;
using DConfig.Components.Repositories;
using DConfig.Components.Models;
using DConfig.Components.Infrastructure;
using Microsoft.Data.Sqlite;
using System.Data;
using System.Security.Cryptography;

namespace DConfigTest
{
    public class ToolRepositoryTest : IDisposable
    {
        private readonly IDbConnection _connection;
        private readonly ToolRepository toolRepository;

        public ToolRepositoryTest()
        {
            //SqliteConnectionFactory erhält einen Speicherort für die .db und implementiert IDbConnectionFactory mit einer CreateConnection-Methode
            var connectionFactory = new SqlServerConnectionFactory("Server = 92.205.188.134, 1433; Initial Catalog = DConfigTest; Persist Security Info = False; User ID = SA; Password = 580=YQc8Tn1:mNdsoJ.8WeLVHMXIqWO2I5; MultipleActiveResultSets = False; Encrypt = False; TrustServerCertificate = True; Connection Timeout = 30; ");

            //Erstellt eine Verbindung zur Datenbank über den Pfad
            _connection = connectionFactory.CreateConnection();

            //Methode um die Tabellen der Datenbank zu erstellen

            //Methode um die Datenbank mit Testdaten zu befüllen

            //Stationrepo mit Verbindung intiialisiert
            toolRepository = new ToolRepository(connectionFactory);
        }




        [Fact]
        public async Task GetToolsFromStation_ReturnsToolsWithGivenStationID()
        {
            // Arrange
            var uniqueShortname = "ToolTest_" + Guid.NewGuid().ToString("N").Substring(0, 6);
            var stationID = 61;

            var newTool = new Tool
            {
                toolShortname = uniqueShortname,
                stationID = stationID,
                toolTypeID = 3,
                lastModified = DateTime.UtcNow
            };

            // Füge neuen Tool-Eintrag über Repository hinzu
            await toolRepository.Add(newTool);

            // Hole den eingefügten Eintrag via SQL, um die ID zu prüfen
            var inserted = await _connection.QuerySingleOrDefaultAsync<Tool>(
                "SELECT TOP 1 * FROM [Tool] WHERE toolShortname = @Name ORDER BY toolID DESC",
                new { Name = uniqueShortname });

            Assert.NotNull(inserted);

            // Act
            var result = (await toolRepository.GetToolsFromStation(stationID)).ToList();

            // Assert
            Assert.NotEmpty(result);
            Assert.Contains(result, t => t.toolID == inserted.toolID);

            // Cleanup
            await toolRepository.Delete(inserted);
        }





        public void Dispose()
        {
            _connection.Dispose();
        }
    }
}

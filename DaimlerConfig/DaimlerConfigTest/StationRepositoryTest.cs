using Xunit;
using Dapper;
using DaimlerConfig.Components.Repositories;
using DaimlerConfig.Components.Models;
using DaimlerConfig.Components.Infrastructure;
using Microsoft.Data.Sqlite;
using System.Data;

namespace DaimlerConfigTest
{
    public class StationRepositoryTest : IDisposable
    {

        private readonly IDbConnection _connection; // Instanz einer DbConnection
        private readonly StationRepository stationRepository; // Instanz eines StationRepos

        public StationRepositoryTest() // Konstruktor
        {
            //SqliteConnectionFactory erhält einen Speicherort für die .db und implementiert IDbConnectionFactory mit einer CreateConnection-Methode
            var connectionFactory = new SqlServerConnectionFactory("Server = 92.205.188.134, 1433; Initial Catalog = DConfigTest; Persist Security Info = False; User ID = SA; Password = 580 = YQc8Tn1:mNdsoJ.8WeLVHMXIqWO2I5; MultipleActiveResultSets = False; Encrypt = False; TrustServerCertificate = True; Connection Timeout = 30; ");

            //Erstellt eine Verbindung zur Datenbank über den Pfad
            _connection = connectionFactory.CreateConnection();

            //Methode um die Datenbank mit Testdaten zu befüllen
            SetUpData();

            //Stationrepo mit Verbindung intiialisiert
            stationRepository = new StationRepository(connectionFactory);
        }


        internal void SetUpData()
        {
            
            _connection.Execute(@"
            INSERT OR IGNORE INTO Line (lineName) VALUES (@lineName1);
            INSERT OR IGNORE INTO Line (lineName) VALUES (@lineName2);",
                new
                {
                    lineName1 = "Line1",
                    lineName2 = "Line2"
                });

            
            _connection.Execute(@"
            INSERT OR IGNORE INTO StationType (stationTypeName) VALUES (@stationTypeName1);
            INSERT OR IGNORE INTO StationType (stationTypeName) VALUES (@stationTypeName2);",
                new
                {
                    stationTypeName1 = "StationType1",
                    stationTypeName2 = "StationType2"
                });
        }




        public void Dispose()
        {
            _connection.Dispose();
        }
    }
}

using Xunit;
using Dapper;
using DaimlerConfig.Components.Repositories;
using DaimlerConfig.Components.Models;
using DaimlerConfig.Components.Infrastructure;
using Microsoft.Data.Sqlite;
using System.Data;

namespace DaimlerConfigTest
{
    public class RepositoryTests : IDisposable
    {
        private readonly IDbConnection _connection;
        private readonly Repository<StationType> _repository;
        private readonly StationRepository stationRepository;

        public RepositoryTests()
        {
            // ConnectionFactory implementiert CreateConnection und wird hier initialisiert
            var connectionFactory = new SqliteConnectionFactory(Path.Combine(Directory.GetCurrentDirectory(), "TestDatenbank.db"));

            // Eine Connection wird hergestellt
            _connection = connectionFactory.CreateConnection();

            // Dummy-Datenbank mit StationType Tabelle erstellen (nur, wenn sie nicht existiert)
            _connection.Execute(@"
                CREATE TABLE IF NOT EXISTS StationType (
                    stationTypeID INTEGER PRIMARY KEY,
                    stationTypeName TEXT NOT NULL
                );
            ");

            // Repository-Instanz erstellen
            _repository = new Repository<StationType>(connectionFactory);
            stationRepository = new StationRepository(connectionFactory);
        }

        [Fact]
        public async void AddStationTypeTest()
        {
            // Instanz von StationType
            var stationType = new StationType
            {
                stationTypeName = "TestType5"
            };

            // Aufruf der Add Methode
            await _repository.Add(stationType);

            // Überprüfung, ob der Datensatz in der Datenbank vorhanden ist
            var result = await _connection.QuerySingleOrDefaultAsync<StationType>(
                "SELECT * FROM StationType WHERE stationTypeName = @stationTypeName",
                new { stationType.stationTypeName });

            // Test 1: Überprüfung, ob der Datensatz existiert 
            Assert.NotNull(result);
            // Test 2: Überprüfung, ob der stationTypeName korrekt ist
            Assert.Equal("TestType5", result.stationTypeName);
        }

        public async void AddStationTest()
        {
            var stationType = new StationType
            {
                stationTypeName = "TestType5"
            };
            await _repository.Add(stationType);

            var station = new Station
            {
                assemblystation = "TestStation",
                stationName = "TestStationName",
                StationType_stationTypeID = 0,
                lastModified = DateTime.Now
            };
            await stationRepository.Add(station);

            var result = await _connection.QuerySingleOrDefaultAsync<Station>(
                "SELECT * FROM Station WHERE stationName = @stationName",
                new { station.stationName });

            Assert.NotNull(result);
            Assert.Equal("TestStationName", result.stationName);
            Assert.Equal("TestStation", result.assemblystation);
            Assert.Equal(1, result.StationType_stationTypeID);
            Assert.NotNull(result.lastModified);

        }

        private void DeleteAllTables()
        {
            // Alle Tabellennamen aus sqlite_master abrufen
            var tableNames = _connection.Query<string>("SELECT name FROM sqlite_master WHERE type='table' AND name NOT LIKE 'sqlite_%';");

            // Jede Tabelle löschen
            foreach (var tableName in tableNames)
            {
                _connection.Execute($"DROP TABLE IF EXISTS {tableName};");
            }
        }

        public void Dispose()
        {
            // Alle Tabellen löschen
            DeleteAllTables();

            // Verbindung schließen
            _connection.Dispose();
        }
    }
}

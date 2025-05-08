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
        }

        [Fact]
        public async void AddTest()
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

        [Fact]
        public async void DeleteTest_Works()
        {
            // Instanz von StationType
            var stationType = new StationType
            {
                stationTypeName = "TestType6"
            };
            // Aufruf der Add Methode
            await _repository.Add(stationType);
            // Überprüfung, ob der Datensatz in der Datenbank vorhanden ist
            var result = await _connection.QuerySingleOrDefaultAsync<StationType>(
                "SELECT * FROM StationType WHERE stationTypeName = @stationTypeName",
                new { stationType.stationTypeName });
            // Test 1: Überprüfung, ob der Datensatz existiert 
            Assert.NotNull(result);
            // Aufruf der Delete Methode
            await _repository.Delete(result);
            // Überprüfung, ob der Datensatz in der Datenbank nicht mehr vorhanden ist
            var deletedResult = await _connection.QuerySingleOrDefaultAsync<StationType>(
                "SELECT * FROM StationType WHERE stationTypeName = @stationTypeName",
                new { stationType.stationTypeName });
            // Test 2: Überprüfung, ob der Datensatz nicht mehr existiert 
            Assert.Null(deletedResult);
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

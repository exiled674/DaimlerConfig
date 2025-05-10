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
            var connectionFactory = new SqliteConnectionFactory(Path.Combine(Directory.GetCurrentDirectory(), "StationTest.db"));

            //Erstellt eine Verbindung zur Datenbank über den Pfad
            _connection = connectionFactory.CreateConnection();

            //Methode um die Tabellen der Datenbank zu erstellen
            CreateTables();

            //Methode um die Datenbank mit Testdaten zu befüllen
            SetUpData();

            //Stationrepo mit Verbindung intiialisiert
            stationRepository = new StationRepository(connectionFactory);
        }

        internal void CreateTables()
        {
            _connection.Execute(@"
                PRAGMA foreign_keys = ON;

                CREATE TABLE IF NOT EXISTS Line (
                  lineID INTEGER PRIMARY KEY AUTOINCREMENT,
                  lineName TEXT NOT NULL UNIQUE
                );  

                CREATE TABLE IF NOT EXISTS StationType (
                  stationTypeID INTEGER PRIMARY KEY AUTOINCREMENT,
                  stationTypeName TEXT NOT NULL UNIQUE
                );

                CREATE TABLE IF NOT EXISTS Station (
                  stationID INTEGER PRIMARY KEY AUTOINCREMENT,
                  assemblystation TEXT NOT NULL,
                  stationName TEXT,
                  stationTypeID INTEGER,
                  lineID INTEGER,
                  lastModified TEXT,
                  FOREIGN KEY (stationTypeID) REFERENCES StationType(stationTypeID)
                  FOREIGN KEY (lineID) REFERENCES Line(lineID)
                );
            ");
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


        [Fact]
        public async void AddStationTest_Works()
        {

            //jetziges Datum
            string zeitpunkt = DateTime.Now.ToString("yyyyMMdd_HHmmssfff");

            //neue Station wird erstellt
            var station = new Station
            {
                lineID = 1,
                assemblystation = "TestStation: " + zeitpunkt,
                stationName = "TestStation: " + zeitpunkt,
                stationTypeID = 1,
                lastModified = DateTime.Now
            };
            
            //Station wird eingefügt
            await stationRepository.Add(station);


            // Abfrage der Station über assemblystation
            var result = await _connection.QueryFirstOrDefaultAsync<Station>(
                "SELECT * FROM Station WHERE assemblystation = @assemblystation",
                new {station.assemblystation});
         
            //Wurde etwas eingefügt?
            Assert.NotNull(result);

            //Sind alle Werte korrekt?
            Assert.Equal(station.lineID, result.lineID);
            Assert.Equal(station.stationName, result.stationName);
            Assert.Equal(station.assemblystation, result.assemblystation);
            Assert.Equal(station.stationTypeID, result.stationTypeID);
            Assert.Equal(station.lastModified?.ToString("yyyy-MM-dd HH:mm:ss"), result.lastModified?.ToString("yyyy-MM-dd HH:mm:ss"));



        }

        [Fact]
        public async void DeleteStationTest_Works()
        {
            //jetziges Datum
            string zeitpunkt = DateTime.Now.ToString("yyyyMMdd_HHmmssfff");

            //Station wird manuell eingefügt
            _connection.Execute(@"
                INSERT INTO Station (lineID, assemblystation, stationName, stationTypeID, lastModified)
                VALUES (@lineID, @assemblystation, @stationName, @stationTypeID, @lastModified);",
                new
                {
                    lineID = 1,
                    assemblystation = "TestStation: " + zeitpunkt,
                    stationName = "TestStation: " + zeitpunkt,
                    stationTypeID = 1,
                    lastModified = DateTime.Now
                });

            //Station wird gesucht 
            var stationToDelete = await _connection.QueryFirstOrDefaultAsync<Station>(
                "SELECT * FROM Station WHERE stationName = @stationName",
                new { stationName = "TestStation: " + zeitpunkt });
            //Wurde etwas gefunden?
            Assert.NotNull(stationToDelete); 

           //Delete wird aufgerufen
            await stationRepository.Delete(stationToDelete);

            //Station wird erneut gesucht
            var deletedStation = await _connection.QueryFirstOrDefaultAsync<Station>(
                "SELECT * FROM Station WHERE stationName = @stationName",
                new { stationName = "TestStation: " + zeitpunkt });
            //Wurde etwas gefunden? (Hoffentlich nicht)
            Assert.Null(deletedStation); 
        }

      
        [Fact]
        public async void UpdateStationTest_Works()
        {
            //jetziges Datum
            string zeitpunkt = DateTime.Now.ToString("yyyyMMdd_HHmmssfff");

            //Station wird manuell eingefügt
            _connection.Execute(@"
                INSERT INTO Station (lineID, assemblystation, stationName, stationTypeID, lastModified)
                VALUES (@lineID, @assemblystation, @stationName, @stationTypeID, @lastModified);",
                new
                {
                    lineID = 1,
                    assemblystation = "TestStation: " + zeitpunkt,
                    stationName = "TestStation: " + zeitpunkt,
                    stationTypeID = 1,
                    lastModified = DateTime.Now
                });

           //Station wird gesucht
            var stationToUpdate = await _connection.QueryFirstOrDefaultAsync<Station>(
                "SELECT * FROM Station WHERE stationName = @stationName",
                new { stationName = "TestStation: " + zeitpunkt });
            //Wurde etwas gefunden?
            Assert.NotNull(stationToUpdate);

            //Neue Werte werden erstellt
            string updatedStationName = "UpdatedStationName_" + DateTime.Now.ToString("yyyyMMdd_HHmmssfff");
            string updatedAssemblyName = "UpdatedAssembly_" + DateTime.Now.ToString("yyyyMMdd_HHmmssfff");

            //Station wird aktualisiert
            stationToUpdate.lineID = 2;
            stationToUpdate.assemblystation = updatedAssemblyName;
            stationToUpdate.stationName = updatedStationName;
            stationToUpdate.stationTypeID = 2;
            stationToUpdate.lastModified = DateTime.Now;

            //Update wird aufgerufen
            await stationRepository.Update(stationToUpdate);

            //Station wird erneut gesucht
            var updatedStation = await _connection.QueryFirstOrDefaultAsync<Station>(
                "SELECT * FROM Station WHERE stationName = @stationName",
                new {  stationToUpdate.stationName });

            //Wurde etwas gefunden?
            Assert.NotNull(updatedStation);

            //Sind alle Werte korrekt?
            Assert.Equal(stationToUpdate.assemblystation, updatedStation.assemblystation);
            Assert.Equal(stationToUpdate.stationName, updatedStation.stationName);
            Assert.Equal(stationToUpdate.stationTypeID, updatedStation.stationTypeID);
            Assert.Equal(stationToUpdate.lineID, updatedStation.lineID);
            Assert.NotNull(updatedStation.lastModified);

            //Nullwerte werden gesetzt
            stationToUpdate.stationName = null;
            stationToUpdate.lastModified = null;

            //Update wird aufgerufen
            await stationRepository.Update(stationToUpdate);

            //Station wird erneut gesucht
            var nullUpdatedStation = await _connection.QueryFirstOrDefaultAsync<Station>(
                "SELECT * FROM Station WHERE stationID = @stationID",
                new { stationToUpdate.stationID });

            //Null-Werte werden überprüft
            Assert.NotNull(nullUpdatedStation);
            Assert.NotNull(nullUpdatedStation.assemblystation);
            Assert.Null(nullUpdatedStation.stationName);
            Assert.Equal(2, nullUpdatedStation.stationTypeID);
            Assert.Null(nullUpdatedStation.lastModified);
        }


        [Fact]
        public async void GetStationByIdTest_Works()
        {
            //jetziges Datum
            string zeitpunkt = DateTime.Now.ToString("yyyyMMdd_HHmmssfff");

            //Station wird manuell eingefügt
            _connection.Execute(@"
            INSERT INTO Station (lineID, assemblystation, stationName, stationTypeID, lastModified)
            VALUES (@lineID, @assemblystation, @stationName, @stationTypeID, @lastModified);",
                new
                {
                    lineID = 1,
                    assemblystation =  zeitpunkt,
                    stationName =  zeitpunkt,
                    stationTypeID = 1,
                    lastModified = DateTime.Now
                });

            //Station wird gesucht
            var stationId = await _connection.QuerySingleAsync<int>(
                "SELECT stationID FROM Station WHERE stationName = @stationName",
                new { stationName =  zeitpunkt });

            //Station wird über Get(ID) gesucht
            var retrievedStation = await stationRepository.Get(stationId);

            //Wurde etwas gefunden?
            Assert.NotNull(retrievedStation);

            //Sind alle Werte korrekt?
            Assert.Equal(zeitpunkt, retrievedStation.assemblystation);
            Assert.Equal(zeitpunkt, retrievedStation.stationName);
            Assert.Equal(1, retrievedStation.stationTypeID);
            Assert.Equal(1, retrievedStation.lineID);
            Assert.NotNull(retrievedStation.lastModified);
        }

        [Fact]
        public async void GetAllStationsTest_Works()
        {
            //Anzahl an Records wird gesucht 
            var expectedCount = await _connection.QuerySingleAsync<int>("SELECT COUNT(*) FROM Station;");

           //Anzahl an Records per Methode gesucht
            var allStations = await stationRepository.GetAll();

           //Wurde das IEnumerable übergeben?
            Assert.NotNull(allStations);
            //Wurde die Anzahl an Records korrekt ermittelt?
            Assert.Equal(expectedCount, allStations.Count());
        }











        public void Dispose()
        {
            _connection.Dispose();
        }
    }
}

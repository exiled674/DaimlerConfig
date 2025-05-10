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

        /*[Fact]
        public async void DeleteStationTest_Works()
        {
            
            string uniqueStationName = DateTime.Now.ToString("yyyyMMdd_HHmmssfff");
            _connection.Execute(@"
                INSERT INTO Station (assemblystation, stationName, StationType_stationTypeID, lastModified)
                VALUES (@assemblystation, @stationName, @StationType_stationTypeID, @lastModified);",
                new
                {
                    assemblystation = "TestAssembly",
                    stationName = uniqueStationName,
                    StationType_stationTypeID = 1,
                    lastModified = DateTime.Now
                });

            
            var stationToDelete = await _connection.QueryFirstOrDefaultAsync<Station>(
                "SELECT * FROM Station WHERE stationName = @stationName",
                new { stationName = uniqueStationName });

            Assert.NotNull(stationToDelete); 

           
            await stationRepository.Delete(stationToDelete);

            
            var deletedStation = await _connection.QueryFirstOrDefaultAsync<Station>(
                "SELECT * FROM Station WHERE stationName = @stationName",
                new { stationName = uniqueStationName });

            Assert.Null(deletedStation); 
        }

      
        [Fact]
        public async void UpdateStationTest_Works()
        {
            
            string initialStationName = DateTime.Now.ToString("yyyyMMdd_HHmmssfff");
            _connection.Execute(@"
                INSERT INTO Station (assemblystation, stationName, StationType_stationTypeID, lastModified)
                VALUES (@assemblystation, @stationName, @StationType_stationTypeID, @lastModified);",
                new
                {
                    assemblystation = "InitialAssembly",
                    stationName = initialStationName,
                    StationType_stationTypeID = 1,
                    lastModified = DateTime.Now
                });

           
            var stationToUpdate = await _connection.QueryFirstOrDefaultAsync<Station>(
                "SELECT * FROM Station WHERE stationName = @stationName",
                new { stationName = initialStationName });
            Assert.NotNull(stationToUpdate);

            
            string updatedStationName = "UpdatedStationName_" + DateTime.Now.ToString("yyyyMMdd_HHmmssfff");
            string updatedAssemblyName = "UpdatedAssembly_" + DateTime.Now.ToString("yyyyMMdd_HHmmssfff");

            stationToUpdate.assemblystation = updatedAssemblyName;
            stationToUpdate.stationName = updatedStationName;
            stationToUpdate.StationType_stationTypeID = 2;
            stationToUpdate.lastModified = DateTime.Now;

            await stationRepository.Update(stationToUpdate);

           
            var updatedStation = await _connection.QueryFirstOrDefaultAsync<Station>(
                "SELECT * FROM Station WHERE stationName = @stationName",
                new { stationName = stationToUpdate.stationName });

            Assert.NotNull(updatedStation);
            Assert.Equal(updatedAssemblyName, updatedStation.assemblystation);
            Assert.Equal(updatedStationName, updatedStation.stationName);
            Assert.Equal(2, updatedStation.StationType_stationTypeID);
            Assert.NotNull(updatedStation.lastModified);

            
            stationToUpdate.assemblystation = null;
            stationToUpdate.stationName = null;
            stationToUpdate.lastModified = null;

            await stationRepository.Update(stationToUpdate);

            
            var nullUpdatedStation = await _connection.QueryFirstOrDefaultAsync<Station>(
                "SELECT * FROM Station WHERE stationID = @stationID",
                new { stationID = stationToUpdate.stationID });

            Assert.NotNull(nullUpdatedStation);
            Assert.Null(nullUpdatedStation.assemblystation);
            Assert.Null(nullUpdatedStation.stationName);
            Assert.Equal(2, nullUpdatedStation.StationType_stationTypeID);
            Assert.Null(nullUpdatedStation.lastModified);
        }


        [Fact]
        public async void GetStationByIdTest_Works()
        {
           
            string uniqueStationName = DateTime.Now.ToString("yyyyMMdd_HHmmssfff");
            _connection.Execute(@"
            INSERT INTO Station (assemblystation, stationName, StationType_stationTypeID, lastModified)
            VALUES (@assemblystation, @stationName, @StationType_stationTypeID, @lastModified);",
                new
                {
                    assemblystation = "TestAssembly",
                    stationName = uniqueStationName,
                    StationType_stationTypeID = 1,
                    lastModified = DateTime.Now
                });

            
            var stationId = await _connection.QuerySingleAsync<int>(
                "SELECT stationID FROM Station WHERE stationName = @stationName",
                new { stationName = uniqueStationName });

           
            var retrievedStation = await stationRepository.Get(stationId);

            
            Assert.NotNull(retrievedStation);
            Assert.Equal("TestAssembly", retrievedStation.assemblystation);
            Assert.Equal(uniqueStationName, retrievedStation.stationName);
            Assert.Equal(1, retrievedStation.StationType_stationTypeID);
            Assert.NotNull(retrievedStation.lastModified);
        }

        [Fact]
        public async void GetAllStationsTest_Works()
        {
            
            var expectedCount = await _connection.QuerySingleAsync<int>("SELECT COUNT(*) FROM Station;");

           
            var allStations = await stationRepository.GetAll();

           
            Assert.NotNull(allStations);
            Assert.Equal(expectedCount, allStations.Count());
        }*/











        public void Dispose()
        {
            _connection.Dispose();
        }
    }
}

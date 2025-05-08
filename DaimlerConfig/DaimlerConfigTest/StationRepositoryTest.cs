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
        private readonly IDbConnection _connection;
        private readonly StationRepository stationRepository;

        public StationRepositoryTest()
        {
            var connectionFactory = new SqliteConnectionFactory(Path.Combine(Directory.GetCurrentDirectory(), "StationTest.db"));
            _connection = connectionFactory.CreateConnection();

            
            _connection.Execute(@"
                PRAGMA foreign_keys = ON;
                CREATE TABLE IF NOT EXISTS StationType (
                    stationTypeID INTEGER PRIMARY KEY,
                    stationTypeName TEXT NOT NULL
                );
                CREATE TABLE IF NOT EXISTS Station (
                  stationID INTEGER PRIMARY KEY AUTOINCREMENT,
                  assemblystation TEXT,
                  stationName TEXT,
                  StationType_stationTypeID INTEGER,
                  lastModified TEXT,
                  FOREIGN KEY (StationType_stationTypeID) REFERENCES StationType(stationTypeID)
                );

               INSERT OR IGNORE INTO StationType (stationTypeName) VALUES
                ('StationType1'),
                ('StationType2');
            ");

           
            stationRepository = new StationRepository(connectionFactory);
        }

        [Fact]
        public async void AddStationTest_Works()
        {
            string name = DateTime.Now.ToString("yyyyMMdd_HHmmssfff");
            var station = new Station
            {
                assemblystation = "TestStation",
                stationName = name,
                StationType_stationTypeID = 1,
                lastModified = DateTime.Now
            };
            await stationRepository.Add(station);

            var result = await _connection.QueryFirstOrDefaultAsync<Station>(
                "SELECT * FROM Station WHERE stationName = @stationName",
                new { station.assemblystation, station.stationName, station.StationType_stationTypeID, station.lastModified });
         
            Assert.NotNull(result);
            Assert.Equal(name, result.stationName);
            Assert.Equal("TestStation", result.assemblystation);
            Assert.Equal(1, result.StationType_stationTypeID);
            Assert.NotNull(result.lastModified);
           

        }

        [Fact]
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
                "SELECT * FROM Station WHERE stationID = @stationID",
                new { stationID = stationToUpdate.stationID });

            Assert.NotNull(updatedStation); 
            Assert.Equal(updatedAssemblyName, updatedStation.assemblystation);
            Assert.Equal(updatedStationName, updatedStation.stationName);
            Assert.Equal(2, updatedStation.StationType_stationTypeID); 
            Assert.NotNull(updatedStation.lastModified);
        }








        public void Dispose()
        {
            _connection.Dispose();
        }
    }
}

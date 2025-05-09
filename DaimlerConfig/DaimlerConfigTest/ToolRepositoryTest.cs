using Xunit;
using Dapper;
using DaimlerConfig.Components.Repositories;
using DaimlerConfig.Components.Models;
using DaimlerConfig.Components.Infrastructure;
using Microsoft.Data.Sqlite;
using System.Data;

namespace DaimlerConfigTest
{
    public class ToolRepositoryTest : IDisposable
    {
        private readonly IDbConnection _connection;
        private readonly ToolRepository toolRepository;

        public ToolRepositoryTest()
        {
            var connectionFactory = new SqliteConnectionFactory(Path.Combine(Directory.GetCurrentDirectory(), "ToolTest.db"));
            _connection = connectionFactory.CreateConnection();

            _connection.Execute(@"
        PRAGMA foreign_keys = ON;

        -- StationType Tabelle erstellen
        CREATE TABLE IF NOT EXISTS StationType (
            stationTypeID INTEGER PRIMARY KEY,
            stationTypeName TEXT NOT NULL
        );

        -- Station Tabelle erstellen
        CREATE TABLE IF NOT EXISTS Station (
            stationID INTEGER PRIMARY KEY AUTOINCREMENT,
            assemblystation TEXT,
            stationName TEXT,
            StationType_stationTypeID INTEGER,
            lastModified TEXT,
            FOREIGN KEY (StationType_stationTypeID) REFERENCES StationType(stationTypeID)
        );

        -- ToolClass Tabelle erstellen
        CREATE TABLE IF NOT EXISTS ToolClass (
            toolClassID INTEGER PRIMARY KEY AUTOINCREMENT,
            toolClassName TEXT NOT NULL UNIQUE
        );

        -- ToolType Tabelle erstellen
        CREATE TABLE IF NOT EXISTS ToolType (
            toolTypeID INTEGER PRIMARY KEY AUTOINCREMENT,
            toolTypeName TEXT NOT NULL UNIQUE,
            ToolClass_toolClassID INTEGER NOT NULL,
            FOREIGN KEY (ToolClass_toolClassID) REFERENCES ToolClass(toolClassID)
        );

        -- Tool Tabelle erstellen
        CREATE TABLE IF NOT EXISTS Tool (
            toolID INTEGER PRIMARY KEY AUTOINCREMENT,
            toolShortname TEXT,
            toolDescription TEXT,
            ToolType_toolTypeID INTEGER,
            Station_stationID INTEGER NOT NULL,
            IPAdresse_Device TEXT,
            ""PLC-Name"" TEXT,
            DB_NoSend TEXT,
            DB_NoReceive TEXT,
            preCheck_Byte INTEGER DEFAULT 0,
            adress_sendDB INTEGER DEFAULT 0,
            adress_receiveDB INTEGER DEFAULT 0,
            lastModified TEXT,
            FOREIGN KEY (ToolType_toolTypeID) REFERENCES ToolType(toolTypeID),
            FOREIGN KEY (Station_stationID) REFERENCES Station(stationID)
        );

        -- StationType Einträge
        INSERT OR IGNORE INTO StationType (stationTypeName) VALUES
        ('StationType1'),
        ('StationType2');

        -- Station Einträge
        INSERT OR IGNORE INTO Station (assemblystation, stationName, StationType_stationTypeID, lastModified) VALUES
        ('Station1', 'StationName1', 1, '2023-10-01'),
        ('Station2', 'StationName2', 2, '2023-10-02');

        -- ToolClass Einträge
        INSERT OR IGNORE INTO ToolClass (toolClassName) VALUES
        ('ToolClass1'),
        ('ToolClass2');

        -- ToolType Einträge
        INSERT OR IGNORE INTO ToolType (toolTypeName, ToolClass_toolClassID) VALUES
        ('ToolType1', 1),
        ('ToolType2', 2);

        -- Tool Einträge
        INSERT OR IGNORE INTO Tool (toolShortname, toolDescription, ToolType_toolTypeID, Station_stationID, IPAdresse_Device, ""PLC-Name"", DB_NoSend, DB_NoReceive, preCheck_Byte, adress_sendDB, adress_receiveDB, lastModified) VALUES
        ('Tool1', 'Description1', 1, 1, '192.168.0.1', 'PLC1', 'DB1', 'DB2', 0, 0, 0, '2023-10-01'),
        ('Tool2', 'Description2', 2, 1, '192.168.0.2', 'PLC2', 'DB3', 'DB4', 1, 1, 1, '2023-10-02'),
        ('Tool3', 'Description3', 1, 2, '192.168.0.3', 'PLC3', 'DB5', 'DB6', 0, 0, 0, '2023-10-03'),
        ('Tool4', 'Description4', 2, 2, '192.168.0.4', 'PLC4', 'DB7', 'DB8', 1, 1, 1, '2023-10-04');
    ");

            toolRepository = new ToolRepository(connectionFactory);
        }


        [Fact]
        public async void GetToolsFromStationTest_Work()
        {

            var expectedCount = await _connection.QuerySingleAsync<int>("SELECT COUNT(*) FROM Tool WHERE Station_stationID = 1;");
            var tools = await toolRepository.getToolsFromStation(1);
            Assert.NotNull(tools);
            Assert.Equal(expectedCount, tools.Count());

        }






        public void Dispose()
        {
            _connection.Dispose();
        }
    }
}

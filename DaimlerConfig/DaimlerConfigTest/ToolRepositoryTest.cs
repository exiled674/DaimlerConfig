using Xunit;
using Dapper;
using DaimlerConfig.Components.Repositories;
using DaimlerConfig.Components.Models;
using DaimlerConfig.Components.Infrastructure;
using Microsoft.Data.Sqlite;
using System.Data;
using System.Security.Cryptography;

namespace DaimlerConfigTest
{
    public class ToolRepositoryTest : IDisposable
    {
        private readonly IDbConnection _connection;
        private readonly ToolRepository toolRepository;

        public ToolRepositoryTest()
        {
            //SqliteConnectionFactory erhält einen Speicherort für die .db und implementiert IDbConnectionFactory mit einer CreateConnection-Methode
            var connectionFactory = new SqliteConnectionFactory(Path.Combine(Directory.GetCurrentDirectory(), "ToolTest.db"));

            //Erstellt eine Verbindung zur Datenbank über den Pfad
            _connection = connectionFactory.CreateConnection();

            //Methode um die Tabellen der Datenbank zu erstellen
            CreateTables();

            //Methode um die Datenbank mit Testdaten zu befüllen
            SetUpData();

            //Stationrepo mit Verbindung intiialisiert
            toolRepository = new ToolRepository(connectionFactory);
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

                CREATE TABLE IF NOT EXISTS ToolClass (
                  toolClassID INTEGER PRIMARY KEY AUTOINCREMENT,
                  toolClassName TEXT NOT NULL UNIQUE
                );

                CREATE TABLE IF NOT EXISTS ToolType (
                  toolTypeID INTEGER PRIMARY KEY AUTOINCREMENT,
                  toolTypeName TEXT NOT NULL UNIQUE,
                  toolClassID INTEGER NOT NULL,
                  FOREIGN KEY (toolClassID) REFERENCES ToolClass(toolClassID)
                );

                CREATE TABLE IF NOT EXISTS Tool (
                  toolID INTEGER PRIMARY KEY AUTOINCREMENT,
                  toolShortname TEXT,
                  toolDescription TEXT,
                  toolTypeID INTEGER,
                  stationID INTEGER NOT NULL,
                  ipAddressDevice TEXT,
                  plcName TEXT,
                  dbNoSend TEXT,
                  dbNoReceive TEXT,
                  preCheckByte INTEGER DEFAULT 0,
                  addressSendDB INTEGER DEFAULT 0,
                  addressReceiveDB INTEGER DEFAULT 0,
                  lastModified TEXT,
                  FOREIGN KEY (toolTypeID) REFERENCES ToolType(toolTypeID),
                  FOREIGN KEY (stationID) REFERENCES Station(stationID)
                );
            ");
        }

        internal void SetUpData()
        {
            // Lines einfügen
            _connection.Execute(@"
                INSERT OR IGNORE INTO Line (lineName) VALUES
                ('Line1'),
                ('Line2');
            ");

                    // StationTypes einfügen
                    _connection.Execute(@"
                INSERT OR IGNORE INTO StationType (stationTypeName) VALUES
                ('StationType1'),
                ('StationType2');
            ");

                    // Stations einfügen
                    _connection.Execute(@"
                INSERT OR IGNORE INTO Station (assemblystation, stationName, stationTypeID, lineID, lastModified) VALUES
                ('AssemblyStation1', 'Station1', 1, 1, '2023-10-01'),
                ('AssemblyStation2', 'Station2', 2, 2, '2023-10-02');
            ");

                    // ToolClasses einfügen
                    _connection.Execute(@"
                INSERT OR IGNORE INTO ToolClass (toolClassName) VALUES
                ('ToolClass1'),
                ('ToolClass2');
            ");

                    // ToolTypes einfügen
                    _connection.Execute(@"
                INSERT OR IGNORE INTO ToolType (toolTypeName, toolClassID) VALUES
                ('ToolType1', 1),
                ('ToolType2', 2);
            ");
        }

        [Fact]
        public async void AddToolTest_Works()
        {
            // Zeitstempel für Eindeutigkeit
            string zeitpunkt = DateTime.Now.ToString("yyyyMMdd_HHmmssfff");

            // Neues Tool erstellen
            var tool = new Tool
            {
                toolShortname = "ToolShort_" + zeitpunkt,
                toolDescription = "Beschreibung_" + zeitpunkt,
                toolTypeID = 1,       // muss existieren in SetupData
                stationID = 1,        // muss existieren in SetupData
                ipAddressDevice = "192.168.0.100",
                plcName = "PLC_" + zeitpunkt,
                dbNoSend = "100",
                dbNoReceive = "101",
                preCheckByte = 1,
                addressSendDB = "a",
                addressReceiveDB = "a",
                lastModified = DateTime.Now
            };

            // Tool speichern
            await toolRepository.Add(tool);

            // Überprüfen, ob das Tool korrekt eingefügt wurde
            var result = await _connection.QueryFirstOrDefaultAsync<Tool>(
                "SELECT * FROM Tool WHERE toolShortname = @toolShortname",
                new { tool.toolShortname });

            // Existenz prüfen
            Assert.NotNull(result);

            // Werte prüfen
            Assert.Equal(tool.toolShortname, result.toolShortname);
            Assert.Equal(tool.toolDescription, result.toolDescription);
            Assert.Equal(tool.toolTypeID, result.toolTypeID);
            Assert.Equal(tool.stationID, result.stationID);
            Assert.Equal(tool.ipAddressDevice, result.ipAddressDevice);
            Assert.Equal(tool.plcName, result.plcName);
            Assert.Equal(tool.dbNoSend, result.dbNoSend);
            Assert.Equal(tool.dbNoReceive, result.dbNoReceive);
            Assert.Equal(tool.preCheckByte, result.preCheckByte);
            Assert.Equal(tool.addressSendDB, result.addressSendDB);
            Assert.Equal(tool.addressReceiveDB, result.addressReceiveDB);
            Assert.Equal(tool.lastModified?.ToString("yyyy-MM-dd HH:mm:ss"), result.lastModified?.ToString("yyyy-MM-dd HH:mm:ss"));
        }



        [Fact]
        public async void DeleteToolTest_Works()
        {
            // Tool manuell einfügen
            string zeitpunkt = DateTime.Now.ToString("yyyyMMdd_HHmmssfff");
            _connection.Execute(@"
        INSERT INTO Tool (toolShortname, toolDescription, toolTypeID, stationID, ipAddressDevice, plcName, dbNoSend, dbNoReceive, preCheckByte, addressSendDB, addressReceiveDB, lastModified)
        VALUES (@toolShortname, @toolDescription, @toolTypeID, @stationID, @ipAddressDevice, @plcName, @dbNoSend, @dbNoReceive, @preCheckByte, @addressSendDB, @addressReceiveDB, @lastModified);",
                new
                {
                    toolShortname = "ToolShort_" + zeitpunkt,
                    toolDescription = "Beschreibung_" + zeitpunkt,
                    toolTypeID = 1,
                    stationID = 1,
                    ipAddressDevice = "192.168.0.100",
                    plcName = "PLC_" + zeitpunkt,
                    dbNoSend = "100",
                    dbNoReceive = "101",
                    preCheckByte = 1,
                    addressSendDB = "a",
                    addressReceiveDB = "a",
                    lastModified = DateTime.Now
                });

            // Tool suchen
            var toolToDelete = await _connection.QueryFirstOrDefaultAsync<Tool>(
                "SELECT * FROM Tool WHERE toolShortname = @toolShortname",
                new { toolShortname = "ToolShort_" + zeitpunkt });
            Assert.NotNull(toolToDelete);

            // Tool löschen
            await toolRepository.Delete(toolToDelete);

            // Überprüfen, ob das Tool gelöscht wurde
            var deletedTool = await _connection.QueryFirstOrDefaultAsync<Tool>(
                "SELECT * FROM Tool WHERE toolShortname = @toolShortname",
                new { toolShortname = "ToolShort_" + zeitpunkt });
            Assert.Null(deletedTool);
        }

        [Fact]
        public async void DeleteAllTools_Works()
        {
            // Mehrere Tools hinzufügen
            await toolRepository.Add(new Tool
            {
                toolShortname = "Tool1",
                toolDescription = "Description1",
                toolTypeID = 1,
                stationID = 1,
                ipAddressDevice = "192.168.0.101",
                plcName = "PLC1",
                dbNoSend = "DB1",
                dbNoReceive = "DB2",
                preCheckByte = 1,
                addressSendDB = "a",
                addressReceiveDB = "b",
                lastModified = DateTime.Now
            });

            await toolRepository.Add(new Tool
            {
                toolShortname = "Tool2",
                toolDescription = "Description2",
                toolTypeID = 2,
                stationID = 2,
                ipAddressDevice = "192.168.0.102",
                plcName = "PLC2",
                dbNoSend = "DB3",
                dbNoReceive = "DB4",
                preCheckByte = 0,
                addressSendDB = "c",
                addressReceiveDB = "d",
                lastModified = DateTime.Now
            });

            // Alle Tools löschen
            var allTools = await toolRepository.GetAll();
            foreach (var tool in allTools)
            {
                await toolRepository.Delete(tool);
            }

            // Überprüfen, ob keine Tools mehr existieren
            var remainingTools = await toolRepository.GetAll();
            Assert.Empty(remainingTools);
        }

        [Fact]
        public async void DeleteNonExistentTool_DoesNotThrow()
        {
            // Tool erstellen, das nicht in der Datenbank existiert
            var nonExistentTool = new Tool
            {
                toolID = 9999, // Nicht existierende ID
                toolShortname = "NonExistentTool",
                toolDescription = "This tool does not exist",
                toolTypeID = 1,
                stationID = 1,
                ipAddressDevice = "192.168.0.200",
                plcName = "NonExistentPLC",
                dbNoSend = "DBX",
                dbNoReceive = "DBY",
                preCheckByte = 0,
                addressSendDB = "x",
                addressReceiveDB = "y",
                lastModified = DateTime.Now
            };

            // Versuch, das nicht existierende Tool zu löschen
            await toolRepository.Delete(nonExistentTool);

            // Keine Ausnahme sollte ausgelöst werden
        }


        [Fact]
        public async void UpdateToolTest_Works()
        {
            // Tool manuell einfügen
            string zeitpunkt = DateTime.Now.ToString("yyyyMMdd_HHmmssfff");
            _connection.Execute(@"
        INSERT INTO Tool (toolShortname, toolDescription, toolTypeID, stationID, ipAddressDevice, plcName, dbNoSend, dbNoReceive, preCheckByte, addressSendDB, addressReceiveDB, lastModified)
        VALUES (@toolShortname, @toolDescription, @toolTypeID, @stationID, @ipAddressDevice, @plcName, @dbNoSend, @dbNoReceive, @preCheckByte, @addressSendDB, @addressReceiveDB, @lastModified);",
                new
                {
                    toolShortname = "ToolShort_" + zeitpunkt,
                    toolDescription = "Beschreibung_" + zeitpunkt,
                    toolTypeID = 1,
                    stationID = 1,
                    ipAddressDevice = "192.168.0.100",
                    plcName = "PLC_" + zeitpunkt,
                    dbNoSend = "100",
                    dbNoReceive = "101",
                    preCheckByte = 1,
                    addressSendDB = "a",
                    addressReceiveDB = "a",
                    lastModified = DateTime.Now
                });

            // Tool suchen
            var toolToUpdate = await _connection.QueryFirstOrDefaultAsync<Tool>(
                "SELECT * FROM Tool WHERE toolShortname = @toolShortname",
                new { toolShortname = "ToolShort_" + zeitpunkt });
            Assert.NotNull(toolToUpdate);

            // Neue Werte für das Tool
            string updatedShortname = "UpdatedToolShort_" + zeitpunkt;
            string updatedDescription = "Updated Beschreibung";
            string updatedIpAddress = "192.168.0.200";
            string updatedPlcName = "UpdatedPLC_" + zeitpunkt;
            string updatedDbNoSend = "200";
            string updatedDbNoReceive = "201";
            int updatedPreCheckByte = 0;
            string updatedAddressSendDB = "b";
            string updatedAddressReceiveDB = "c";
            DateTime updatedLastModified = DateTime.Now;

            // Tool aktualisieren
            toolToUpdate.toolShortname = updatedShortname;
            toolToUpdate.toolDescription = updatedDescription;
            toolToUpdate.toolTypeID = 2; // Ändern auf einen anderen ToolType
            toolToUpdate.stationID = 2;  // Ändern auf eine andere Station
            toolToUpdate.ipAddressDevice = updatedIpAddress;
            toolToUpdate.plcName = updatedPlcName;
            toolToUpdate.dbNoSend = updatedDbNoSend;
            toolToUpdate.dbNoReceive = updatedDbNoReceive;
            toolToUpdate.preCheckByte = updatedPreCheckByte;
            toolToUpdate.addressSendDB = updatedAddressSendDB;
            toolToUpdate.addressReceiveDB = updatedAddressReceiveDB;
            toolToUpdate.lastModified = updatedLastModified;

            await toolRepository.Update(toolToUpdate);

            // Überprüfen, ob das Tool aktualisiert wurde
            var updatedTool = await _connection.QueryFirstOrDefaultAsync<Tool>(
                "SELECT * FROM Tool WHERE toolID = @toolID",
                new { toolToUpdate.toolID });
            Assert.NotNull(updatedTool);

            // Werte prüfen
            Assert.Equal(updatedShortname, updatedTool.toolShortname);
            Assert.Equal(updatedDescription, updatedTool.toolDescription);
            Assert.Equal(2, updatedTool.toolTypeID); // Geänderter ToolType
            Assert.Equal(2, updatedTool.stationID);  // Geänderte Station
            Assert.Equal(updatedIpAddress, updatedTool.ipAddressDevice);
            Assert.Equal(updatedPlcName, updatedTool.plcName);
            Assert.Equal(updatedDbNoSend, updatedTool.dbNoSend);
            Assert.Equal(updatedDbNoReceive, updatedTool.dbNoReceive);
            Assert.Equal(updatedPreCheckByte, updatedTool.preCheckByte);
            Assert.Equal(updatedAddressSendDB, updatedTool.addressSendDB);
            Assert.Equal(updatedAddressReceiveDB, updatedTool.addressReceiveDB);
            Assert.Equal(updatedLastModified.ToString("yyyy-MM-dd HH:mm:ss"), updatedTool.lastModified?.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        [Fact]
        public async void GetToolByIdTest_Works()
        {
            // Tool manuell einfügen
            string zeitpunkt = DateTime.Now.ToString("yyyyMMdd_HHmmssfff");
            _connection.Execute(@"
        INSERT INTO Tool (toolShortname, toolDescription, toolTypeID, stationID, ipAddressDevice, plcName, dbNoSend, dbNoReceive, preCheckByte, addressSendDB, addressReceiveDB, lastModified)
        VALUES (@toolShortname, @toolDescription, @toolTypeID, @stationID, @ipAddressDevice, @plcName, @dbNoSend, @dbNoReceive, @preCheckByte, @addressSendDB, @addressReceiveDB, @lastModified);",
                new
                {
                    toolShortname = "ToolShort_" + zeitpunkt,
                    toolDescription = "Beschreibung_" + zeitpunkt,
                    toolTypeID = 1,
                    stationID = 1,
                    ipAddressDevice = "192.168.0.100",
                    plcName = "PLC_" + zeitpunkt,
                    dbNoSend = "100",
                    dbNoReceive = "101",
                    preCheckByte = 1,
                    addressSendDB = "a",
                    addressReceiveDB = "a",
                    lastModified = DateTime.Now
                });

            // Tool-ID abrufen
            var toolId = await _connection.QuerySingleAsync<int>(
                "SELECT toolID FROM Tool WHERE toolShortname = @toolShortname",
                new { toolShortname = "ToolShort_" + zeitpunkt });

            // Tool über Repository abrufen
            var retrievedTool = await toolRepository.Get(toolId);

            // Überprüfen, ob das Tool gefunden wurde
            Assert.NotNull(retrievedTool);
            Assert.Equal(toolId, retrievedTool.toolID);
        }


        [Fact]
        public async void GetAllToolsTest_Works()
        {
            // Anzahl der Tools in der Datenbank abrufen
            var expectedCount = await _connection.QuerySingleAsync<int>("SELECT COUNT(*) FROM Tool;");

            // Tools über Repository abrufen
            var allTools = await toolRepository.GetAll();

            // Überprüfen, ob die Anzahl übereinstimmt
            Assert.NotNull(allTools);
            Assert.Equal(expectedCount, allTools.Count());
        }








        /* [Fact]
         public async void GetToolsFromStationTest_Work()
         {
             // Korrigierte Abfrage mit stationID
             var expectedCount = await _connection.QuerySingleAsync<int>("SELECT COUNT(*) FROM Tool WHERE stationID = 1;");
             var tools = await toolRepository.getToolsFromStation(1);
             Assert.NotNull(tools);
             Assert.Equal(expectedCount, tools.Count());
         }*/







        public void Dispose()
        {
            _connection.Dispose();
        }
    }
}

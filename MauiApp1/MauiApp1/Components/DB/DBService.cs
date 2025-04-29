using Microsoft.Data.Sqlite;
using Microsoft.Maui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;


namespace MauiApp1.Components.DB
{
    public class DBService
    {
        private string _connectionString = $"Data Source={Path.Combine(Directory.GetCurrentDirectory(), "meineDatenbank.db")}";  // SQLite-Verbindung zur Datei

        // Methode zum Erstellen der Datenbank und Tabellen
        public void CreateDatabase()
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                // SQL-Abfragen für das Erstellen der Tabellen
                var createTableQuery = @"
                

                CREATE TABLE IF NOT EXISTS ""StationType"" (
                  ""idStationType"" INTEGER PRIMARY KEY AUTOINCREMENT,
                  ""stationTypeName"" TEXT UNIQUE
                );

                INSERT OR IGNORE INTO ""StationType"" (stationTypeName) VALUES
                ('StationController'),
                ('PCS-Service'),
                ('DataPump');

                CREATE TABLE IF NOT EXISTS ""Station"" (
                  ""stationID"" INTEGER PRIMARY KEY,
                  ""stationNumber"" TEXT UNIQUE,
                  ""stationDescription"" TEXT,
                  ""StationType_idStationType"" INTEGER,
                  FOREIGN KEY (""StationType_idStationType"") REFERENCES ""StationType"" (""idStationType"")
                );

                CREATE TABLE IF NOT EXISTS ""ToolClass"" (
                  ""idToolClass"" INTEGER PRIMARY KEY,
                  ""toolClassName"" TEXT UNIQUE
                );

                CREATE TABLE IF NOT EXISTS ""ToolType"" (
                  ""idToolType"" INTEGER PRIMARY KEY,
                  ""ToolClass_idToolClass"" INTEGER,
                  ""toolTypeName"" TEXT UNIQUE,
                  FOREIGN KEY (""ToolClass_idToolClass"") REFERENCES ""ToolClass"" (""idToolClass"")
                );

                CREATE TABLE IF NOT EXISTS ""Tool"" (
                  ""toolID"" INTEGER,
                  ""Station_stationID"" INTEGER,
                  ""toolShortname"" TEXT,
                  ""toolDescription"" TEXT,
                  ""ToolClass_idToolClass"" INTEGER,
                  ""ToolType_idToolType"" INTEGER,
                  PRIMARY KEY (""toolID"", ""Station_stationID""),
                  FOREIGN KEY (""Station_stationID"") REFERENCES ""Station"" (""stationID""),
                  FOREIGN KEY (""ToolClass_idToolClass"") REFERENCES ""ToolClass"" (""idToolClass""),
                  FOREIGN KEY (""ToolType_idToolType"") REFERENCES ""ToolType"" (""idToolType"")
                );


                



                CREATE TABLE IF NOT EXISTS ""qGate"" (
                  ""idQGate"" INTEGER PRIMARY KEY,
                  ""qGateName"" TEXT
                );

                CREATE TABLE IF NOT EXISTS ""DecisionClass"" (
                  ""idDecisionClass"" INTEGER PRIMARY KEY,
                  ""decisionClassName"" TEXT
                );

                CREATE TABLE IF NOT EXISTS ""SavingClass"" (
                  ""idSavingClass"" INTEGER PRIMARY KEY,
                  ""savingClassName"" TEXT
                );

                CREATE TABLE IF NOT EXISTS ""GenerationClass"" (
                  ""idGenerationClass"" INTEGER PRIMARY KEY,
                  ""generationClassName"" TEXT
                );

                CREATE TABLE IF NOT EXISTS ""VerificationClass"" (
                  ""idVerificationClass"" INTEGER PRIMARY KEY,
                  ""verificationClassName"" TEXT
                );

                CREATE TABLE IF NOT EXISTS ""Operation"" (
                  ""idOperation"" INTEGER,
                  ""Tool_toolID"" INTEGER,
                  ""Station_stationID"" INTEGER,
                  ""operationShortname"" TEXT,
                  ""operationDescription"" TEXT,
                  ""operationSequenceGroup"" TEXT,
                  ""operationSequence"" TEXT,
                  ""operationDecisionCriteria"" TEXT,
                  ""parallel"" INTEGER,
                  ""qGate_idQGate"" INTEGER,
                  ""alwaysPerform"" INTEGER,
                  ""Operationcol"" TEXT,
                  ""DecisionClass_idDecisionClass"" INTEGER,
                  ""SavingClass_idSavingClass"" INTEGER,
                  ""GenerationClass_idGenerationClass"" INTEGER,
                  ""VerificationClass_idVerificationClass"" INTEGER,
                  PRIMARY KEY (""idOperation""),
                  FOREIGN KEY (""Tool_toolID"", ""Station_stationID"") REFERENCES ""Tool"" (""toolID"", ""Station_stationID""),
                  FOREIGN KEY (""qGate_idQGate"") REFERENCES ""qGate"" (""idQGate""),
                  FOREIGN KEY (""DecisionClass_idDecisionClass"") REFERENCES ""DecisionClass"" (""idDecisionClass""),
                  FOREIGN KEY (""SavingClass_idSavingClass"") REFERENCES ""SavingClass"" (""idSavingClass""),
                  FOREIGN KEY (""GenerationClass_idGenerationClass"") REFERENCES ""GenerationClass"" (""idGenerationClass""),
                  FOREIGN KEY (""VerificationClass_idVerificationClass"") REFERENCES ""VerificationClass"" (""idVerificationClass"")
                );

                

                PRAGMA foreign_keys = on;
                ";

                // Führen der Abfragen aus
                connection.Execute(createTableQuery);
            }
        }

        public void InsertSomething()
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                // 1. Dummy-Daten für Tools einfügen
                var insertTools = @"
                INSERT OR REPLACE INTO Tool (toolID, Station_stationID, toolShortname, toolDescription) VALUES
                (1, 1, 'shortname 1', 'desc 1'),
                (2, 3, 'shortname 2', 'desc 2'),
                (2, 2, 'shortname 2', 'desc 2');";

                connection.Execute(insertTools);

                var insertOperations = @"
                INSERT OR REPLACE INTO Operation (idOperation, Tool_toolID, Station_stationID, operationDescription)
                VALUES 
                (1, 1, 1, 'Operation 1 Beschreibung'),
                (3, 1, 1, 'Operation 3'),
                
                (2, 2, 2, 'Operation 2 Beschreibung');";
                
                  

                connection.Execute(insertOperations);
            }
        }


        public List<string> GetAllStationNames()
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                // SQL-Abfrage, um nur die stationTypeNames aus der Station-Tabelle abzurufen
                var query = "SELECT stationNumber FROM Station";

                // Ausführen der Abfrage und Rückgabe der Liste der Stationennamen
                var stationNames = connection.Query<string>(query).ToList();

                return stationNames;
            }
        }


       




        public void addNewStation(int idStationType, string stationNumberNew, string stationDescriptionNew)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                var insertStation = @"
                INSERT OR REPLACE INTO Station
                (stationNumber, stationDescription, StationType_idStationType)
                VALUES
                (@stationNumber, @stationDescription, @stationType_idStationType);";

                connection.Execute(insertStation, new {stationNumber = stationNumberNew, stationDescription = stationDescriptionNew, stationType_idStationType = idStationType });


            }
        }




        public List<string> GetToolsFromStation(int stationID)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                // SQL-Abfrage: Alle Tool-Shortnames holen, die zur Station gehören
                var query = @"
                SELECT toolShortname
                FROM Tool
                WHERE Station_stationID = @stationID;";

                var tools = connection.Query<string>(query, new { stationID }).ToList();

                return tools;
            }
        }



        public List<(string operationDescription, int toolID)> GetOperationsFromStation(int stationID)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                // 1. Tools der Station abrufen
                var tools = GetToolsFromStation(stationID);

                if (tools.Count == 0)
                    return new List<(string, int)>(); // Falls keine Tools → keine Operationen

                // 2. Operationen abrufen, die mit den Tools zusammenhängen
                var query = @"
            SELECT operationDescription, Tool_toolID
            FROM Operation
            WHERE Station_stationID = @stationID;";

                var operations = connection.Query<(string, int)>(query, new { stationID }).ToList();

                return operations;
            }
        }

        public int StationIDfromName(string stationName)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                // SQL-Abfrage, um die stationID basierend auf dem stationNumber (Name) zu suchen
                var query = "SELECT stationID FROM Station WHERE stationNumber = @stationName";

                // Die stationID wird als Int zurückgegeben
                var stationID = connection.QuerySingleOrDefault<int>(query, new { stationName });

                return stationID;
            }
        }

        public (string toolShortname, string toolDescription) GetToolInfo(int toolID)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                var query = @"
                SELECT toolShortname, toolDescription
                FROM Tool
                WHERE toolID = @toolID
                LIMIT 1;";  // nur ein Ergebnis

                var result = connection.QuerySingleOrDefault<(string toolShortname, string toolDescription)>(query, new { toolID });

                return result;
            }
        }


        public void InsertTool(
    int toolID,
    int stationID,
    string toolShortname,
    string toolDescription,
    int? toolClassID = null,
    int? toolTypeID = null)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            // sanity-check: station must exist
            var stationExists = connection.ExecuteScalar<int>(
                "SELECT COUNT(*) FROM Station WHERE stationID = @stationID",
                new { stationID }
            );
            if (stationExists == 0)
                throw new InvalidOperationException($"Station #{stationID} does not exist.");

            // now insert with potentially NULL class/type
            var sql = @"
        INSERT OR REPLACE INTO Tool
        (toolID, Station_stationID, toolShortname, toolDescription, ToolClass_idToolClass, ToolType_idToolType)
        VALUES
        (@toolID, @stationID, @toolShortname, @toolDescription, @toolClassID, @toolTypeID);
    ";
            connection.Execute(sql, new
            {
                toolID,
                stationID,
                toolShortname,
                toolDescription,
                toolClassID,
                toolTypeID
            });
        }



        public int GetNextToolID()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            // take the current max(toolID) (or 0 if empty) and add 1
            var sql = "SELECT COALESCE(MAX(toolID), 0) + 1 FROM Tool";
            return connection.ExecuteScalar<int>(sql);
        }


        public void InsertToolWithBlankOp(
    int toolID,
    int stationID,
    string toolShortname,
    string toolDescription,
    int? toolClassID = null,
    int? toolTypeID = null)
        {
            // 1) insert the Tool row
            using (var conn = new SqliteConnection(_connectionString))
            {
                conn.Open();
                var insertToolSql = @"
            INSERT OR REPLACE INTO Tool
              (toolID,
               Station_stationID,
               toolShortname,
               toolDescription,
               ToolClass_idToolClass,
               ToolType_idToolType)
            VALUES
              (@toolID,
               @stationID,
               @toolShortname,
               @toolDescription,
               @toolClassID,
               @toolTypeID);
        ";
                conn.Execute(insertToolSql, new
                {
                    toolID,
                    stationID,
                    toolShortname,
                    toolDescription,
                    toolClassID,
                    toolTypeID
                });

                // 2) insert one blank Operation row for that tool
                var nextOpId = conn.ExecuteScalar<int>(
                    "SELECT COALESCE(MAX(idOperation), 0) + 1 FROM Operation;"
                );

                var insertOpSql = @"
            INSERT INTO Operation
              (idOperation,
               Tool_toolID,
               Station_stationID,
               operationDescription)
            VALUES
              (@idOp,
               @toolID,
               @stationID,
               '');
        ";
                conn.Execute(insertOpSql, new
                {
                    idOp = nextOpId,
                    toolID,
                    stationID
                });
            }
        }




    }
}

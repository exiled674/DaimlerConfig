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
                  PRIMARY KEY (""idOperation"", ""Tool_toolID""),
                  FOREIGN KEY (""Tool_toolID"") REFERENCES ""Tool"" (""toolID""),
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

                // 1. Eltern-Datensatz in StationType einfügen (falls noch nicht vorhanden)
                var insertType = @"
            INSERT OR IGNORE INTO StationType (idStationType, stationTypeName)
            VALUES (@id, @name);";
                connection.Execute(insertType, new { id = 3, name = "DefaultType1" });

                // 2. Datensatz in Station einfügen
                var insertStation = @"
            INSERT INTO Station
              (stationID, stationNumber, stationDescription, StationType_idStationType)
            VALUES
              (@stationID, @stationNumber, @stationDescription, @stationType_idStationType);";
                var parameters = new
                {
                    stationID = 5,
                    stationNumber = "ST",
                    stationDescription = "Beispielsta",
                    stationType_idStationType = 3
                };
                connection.Execute(insertStation, parameters);
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


    }
}

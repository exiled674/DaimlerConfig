using Microsoft.Data.Sqlite;
using Dapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MauiApp1.Components.DB {

    public class DBService
    {
        private string _connectionString = $"Data Source={Path.Combine(Directory.GetCurrentDirectory(), "meinDatenbank.db")}";  // SQLite-Verbindung zur Datei

        // Methode zum Erstellen der Datenbank und Tabellen
        public void CreateDatabase()
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                // SQL-Abfragen für das Erstellen der Tabellen
                var createTableQuery = @"
                CREATE TABLE IF NOT EXISTS Stations (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL
                );
                CREATE TABLE IF NOT EXISTS Tools (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    StationId INTEGER,
                    Configuration TEXT,
                    FOREIGN KEY (StationId) REFERENCES Stations(Id)
                );
                CREATE TABLE IF NOT EXISTS Operations (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    ToolId INTEGER,
                    FOREIGN KEY (ToolId) REFERENCES Tools(Id)
                );";

                // Führen der Abfragen aus
                connection.Execute(createTableQuery);
            }
        }

        // Methode zum Befüllen der Datenbank mit Beispiel-Daten
        public void SeedData()
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                // SQL-Abfragen zum Einfügen der Daten
                var insertQuery = @"
                INSERT INTO Stations (Name) VALUES 
                ('Station 1'),
                ('Station 2'),
                ('Station 3');
                
                INSERT INTO Tools (Name, StationId, Configuration) VALUES 
                ('Tool 1', 1, '{"type": "Drill"}'),
                    ('Tool 2', 1, '{"type": "Welding"}'),
                ('Tool 3', 2, '{"type": "Cutting"}');

                INSERT INTO Operations(Name, ToolId) VALUES
                ('Operation 1', 1),
                ('Operation 2', 2),
                ('Operation 3', 3);
                ";

            // Führen der Abfragen aus
            connection.Execute(insertQuery);
            }
        }

       
    }



}



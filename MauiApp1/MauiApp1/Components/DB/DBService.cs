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
    }
}

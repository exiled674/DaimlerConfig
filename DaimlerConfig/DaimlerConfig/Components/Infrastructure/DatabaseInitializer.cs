using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace DaimlerConfig.Components.Infrastructure
{
    public class DatabaseInitializer
    {
        private readonly IDbConnectionFactory _factory;

        public DatabaseInitializer(IDbConnectionFactory factory)
            => _factory = factory;

        public void EnsureCreated()
        {
            using var conn = _factory.CreateConnection();
            conn.Open();

            var ddl = @"
            PRAGMA foreign_keys = ON;
            CREATE TABLE IF NOT EXISTS Station (
                stationID INTEGER PRIMARY KEY AUTOINCREMENT,
                stationNumber TEXT UNIQUE,
                stationDescription TEXT,
                StationType_idStationType INTEGER
            );
            ";

            conn.Execute(ddl);
        }
    }
}

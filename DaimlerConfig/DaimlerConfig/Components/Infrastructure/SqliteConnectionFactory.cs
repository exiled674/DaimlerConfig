using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace DaimlerConfig.Components.Infrastructure
{
    public class SqliteConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public SqliteConnectionFactory(string databasePath)
        {
            _connectionString = $"Data Source={databasePath}";
        }

        public IDbConnection CreateConnection()
            => new SqliteConnection(_connectionString);
    }
}

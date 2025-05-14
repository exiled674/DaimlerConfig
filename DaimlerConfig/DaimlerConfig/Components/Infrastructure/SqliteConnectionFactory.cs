using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Microsoft.Data.SqlClient;

namespace DaimlerConfig.Components.Infrastructure
{
    public class SqlServerConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public SqlServerConnectionFactory(string connectionString)
            => _connectionString = connectionString;

        public IDbConnection CreateConnection()
            => new SqlConnection(_connectionString);
    }
}
using System.Data;
using Dapper;
using DaimlerConfig.Components.Infrastructure;
using DaimlerConfig.Components.Models;

namespace DaimlerConfig.Components.Repositories
{
    public class ToolRepository : Repository<Tool>, IToolRepository
    {
        public ToolRepository(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory)
        {
        }

        public async Task<IEnumerable<Tool>> GetToolsFromStation(int stationID)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

           
            var query = @"
                SELECT * 
                FROM Tool
                WHERE stationID = @stationID";

            
            var tools = await connection.QueryAsync<Tool>(query, new { stationID });

            return tools;
        }
    }
}

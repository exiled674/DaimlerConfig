using DaimlerConfig.Components.Infrastructure;
using DaimlerConfig.Components.Models;
using Dapper;

namespace DaimlerConfig.Components.Repositories
{
    public class OperationRepository : Repository<Operation>, IOperationRepository
    {
        public OperationRepository(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory)
        {
        }

        public async Task<IEnumerable<Operation>> GetOperationsFromTool(int? toolID)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            var query = @"
                SELECT * 
                FROM [Operation]
                WHERE [toolID] = @toolID";

            var operations = await connection.QueryAsync<Operation>(query, new { toolID });

            return operations;
        }
    }
}
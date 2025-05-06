using DaimlerConfig.Components.Infrastructure;
using DaimlerConfig.Components.Models;

namespace DaimlerConfig.Components.Repositories
{
    public class ToolRepository : Repository<Tool>, IToolRepository
    {
        public ToolRepository(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory)
        {
        }

        public Task<IEnumerable<Tool>> GetToolsFromStation(int stationId)
        {
            throw new NotImplementedException();
        }
    }
}

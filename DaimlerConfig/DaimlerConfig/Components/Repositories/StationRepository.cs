using DaimlerConfig.Components.Infrastructure;
using DaimlerConfig.Components.Models;

namespace DaimlerConfig.Components.Repositories
{
    public class StationRepository : Repository<Station>, IStationRepository
    {
        public StationRepository(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory)
        {
        }
    }
}

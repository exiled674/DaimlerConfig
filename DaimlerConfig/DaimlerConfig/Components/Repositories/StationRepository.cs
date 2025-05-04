using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DaimlerConfig.Components.Models;

namespace DaimlerConfig.Components.Repositories
{
    public class StationRepository : Repository<Station>, IStationRepository
    {
        public Task<IEnumerable<Station>> GetAllStations()
        {
            throw new NotImplementedException();
        }
    }
}

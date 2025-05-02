using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DaimlerConfig.Components.Models;

namespace DaimlerConfig.Components.Repositories
{
    public class ToolRepository : Repository<Tool>, IToolRepository
    {
        public Task<IEnumerable<Tool>> getToolsFromStation(int stationID)
        {
            throw new NotImplementedException();
        }
    }
}

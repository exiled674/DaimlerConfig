using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DConfig.Components.Models;

namespace DConfig.Components.Repositories
{
    public interface IToolRepository : IRepository<Tool>
    {
        Task<IEnumerable<Tool>> GetToolsFromStation(int? stationID);


    }
}

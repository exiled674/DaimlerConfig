using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DConfig.Components.Models;

namespace DConfig.Components.Repositories
{
    public interface IStationRepository : IRepository<Station>
    {
        public  Task<IEnumerable<Station>> GetStationsFromLine(int? lineID);

    }
}

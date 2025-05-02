using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DaimlerConfig.Components.Models;

namespace DaimlerConfig.Components.Repositories
{
    public interface IOperationRepository : IRepository<Operation>
    {
        public Task<IEnumerable<Operation>> GetOperationsFromTool(int toolID);
    }
}

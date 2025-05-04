using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DaimlerConfig.Components.Infrastructure;
using DaimlerConfig.Components.Models;

namespace DaimlerConfig.Components.Repositories
{
    public class OperationRepository : Repository<Operation>, IOperationRepository
    {
        public OperationRepository(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory)
        {
        }

        public Task<IEnumerable<Operation>> GetOperationsFromTool(int toolID)
        {
            throw new NotImplementedException();
        }
    }
}

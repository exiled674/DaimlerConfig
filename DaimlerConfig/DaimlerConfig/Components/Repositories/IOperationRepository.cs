using DaimlerConfig.Components.Models;

namespace DaimlerConfig.Components.Repositories
{
    public interface IOperationRepository : IRepository<Operation>
    {
        public Task<IEnumerable<Operation>> GetOperationsFromTool(int toolId);
    }
}

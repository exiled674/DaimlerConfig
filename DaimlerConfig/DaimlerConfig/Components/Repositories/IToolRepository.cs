using DaimlerConfig.Components.Models;

namespace DaimlerConfig.Components.Repositories
{
    public interface IToolRepository : IRepository<Tool>
    {
        Task<IEnumerable<Tool>> GetToolsFromStation(int stationId);


    }
}

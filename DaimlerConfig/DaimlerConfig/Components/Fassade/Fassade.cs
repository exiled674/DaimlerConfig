using DaimlerConfig.Components.Repositories;
using DaimlerConfig.Components.Models;



namespace DaimlerConfig.Components.Fassade
{
    public class Fassade
    {
        public IToolRepository ToolRepository { get; private set; }
        public IOperationRepository OperationRepository { get; private set; }
        public IStationRepository StationRepository { get; private set; }
        public IRepository<Line> LineRepository { get; private set; }

        public Fassade(IToolRepository toolRepository, IOperationRepository operationRepository, IStationRepository stationRepository, IRepository<Line> lineRepository)
        {
            ToolRepository = toolRepository;
            OperationRepository = operationRepository;
            StationRepository = stationRepository;
            LineRepository = lineRepository;
        }

        #region Line
        public async Task<IEnumerable<Line>> GetAllLinesAsync()
        {
            return await LineRepository.GetAll();
        }

        public async Task<IEnumerable<Line>> GetAllLinesSortedByDate()
        {
            return await LineRepository.getAllOrderedByDate();
        }

        public async Task<Line?> GetLineByName(string lineName)
        {
            return await LineRepository.GetByName(lineName);
        }

        public async Task AddLine(Line line)
        {
            await LineRepository.Add(line);
        }

        public async Task DeleteLine(Line line)
        {
            await LineRepository.Delete(line);
        }

        public async Task<bool> LineExistsByName(string name)
        {
            return await LineRepository.ExistsByName(name);
        }

        public async Task UpdateLine(Line line)
        {
            await LineRepository.Update(line);
        }
        #endregion

        #region Station
        public async Task<IEnumerable<Station>> GetStationsFromLine(int lineID)
        {
            return await StationRepository.GetStationsFromLine(lineID);
        }

        public async Task UpdateStation(Station station)
        {
            await StationRepository.Update(station);
        }

        public async Task AddStation(Station station)
        {
            await StationRepository.Add(station);
        }

        public async Task<bool> StationExistsByName(string name)
        {
            return await StationRepository.ExistsByName(name);
        }
        #endregion

        #region Tool
        public async Task<IEnumerable<Tool>> GetToolsFromStation(int stationID)
        {
            return await ToolRepository.GetToolsFromStation(stationID);
        }

        public async Task UpdateTool(Tool tool)
        {
            await ToolRepository.Update(tool);
        }

        public async Task AddTool(Tool tool)
        {
            await ToolRepository.Add(tool);
        }

        public async Task<bool> ToolExistsByName(string name)
        {
            return await ToolRepository.ExistsByName(name);
        }
        #endregion

        #region Operation
        public async Task<IEnumerable<Operation>> GetOperationsFromTool(int toolID)
        {
            return await OperationRepository.GetOperationsFromTool(toolID);
        }

        public async Task<IEnumerable<Operation>> GetAllOperations()
        {
            return await OperationRepository.GetAll();
        }

        public async Task UpdateOperation(Operation operation)
        {
            await OperationRepository.Update(operation);
        }

        public async Task AddOperation(Operation operation)
        {
            await OperationRepository.Add(operation);
        }

        public async Task<bool> OperationExistsByName(string name)
        {
            return await OperationRepository.ExistsByName(name);
        }
        #endregion
    }
}

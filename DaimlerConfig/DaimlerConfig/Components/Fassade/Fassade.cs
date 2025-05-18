using DaimlerConfig.Components.Repositories;
using DaimlerConfig.Components.Models;
using DaimlerConfig.Components.JsonHandler;



namespace DaimlerConfig.Components.Fassade
{
    public class Fassade
    {
        public IToolRepository ToolRepository { get; private set; }
        public IOperationRepository OperationRepository { get; private set; }
        public IStationRepository StationRepository { get; private set; }
        public IRepository<Line> LineRepository { get; private set; }

        private readonly WriteJson _writeJson = new WriteJson();

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
            var line = await LineRepository.Get(station.lineID);
            if (line != null)
            {
                line.lastModified = DateTime.Now;
                await LineRepository.Update(line);
            }
        }

        public async Task AddStation(Station station)
        {
            await StationRepository.Add(station);
            var line = await LineRepository.Get(station.lineID);
            if (line != null)
            {
                line.lastModified = DateTime.Now;
                await LineRepository.Update(line);
            }
        }

        public async Task<bool> StationExistsByName(string name)
        {
            return await StationRepository.ExistsByName(name);
        }

        public async Task DeleteStation(Station station)
        {
            await StationRepository.Delete(station);
            var line = await LineRepository.Get(station.lineID);
            if (line != null)
            {
                line.lastModified = DateTime.Now;
                await LineRepository.Update(line);
            }
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
            var station = await StationRepository.Get(tool.stationID);
            if (station != null)
            {
                station.lastModified = DateTime.Now;
                await StationRepository.Update(station);
            }
            var line = await LineRepository.Get(station.lineID);
            if (line != null)
            {
                line.lastModified = DateTime.Now;
                await LineRepository.Update(line);
            }
        }

        public async Task AddTool(Tool tool)
        {
            await ToolRepository.Add(tool);
            var station = await StationRepository.Get(tool.stationID);
            if (station != null)
            {
                station.lastModified = DateTime.Now;
                await StationRepository.Update(station);
            }
            var line = await LineRepository.Get(station.lineID);
            if (line != null)
            {
                line.lastModified = DateTime.Now;
                await LineRepository.Update(line);
            }
        }

        public async Task<bool> ToolExistsByName(string name)
        {
            return await ToolRepository.ExistsByName(name);
        }

        public async Task DeleteTool(Tool tool)
        {
            await ToolRepository.Delete(tool);
            var station = await StationRepository.Get(tool.stationID);
            if (station != null)
            {
                station.lastModified = DateTime.Now;
                await StationRepository.Update(station);
            }
            var line = await LineRepository.Get(station.lineID);
            if (line != null)
            {
                line.lastModified = DateTime.Now;
                await LineRepository.Update(line);
            }
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

        public async Task DeleteOperation(Operation operation)
        {
            await OperationRepository.Delete(operation);
        }
        #endregion

        public async Task<string> Export()
        {
            var stations = await StationRepository.GetAll();
            var tools = await ToolRepository.GetAll();
            var operations = await OperationRepository.GetAll();

            var stationList = stations.ToList();
            var toolList = tools.ToList();
            var operationList = operations.ToList();

            return await _writeJson.WriteAllToFileAsync(stationList, toolList, operationList);
        }
    }
}

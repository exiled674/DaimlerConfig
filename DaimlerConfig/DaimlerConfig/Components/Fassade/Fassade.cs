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

        public IRepository<StationType> StationTypeRepository { get; private set; }

        private readonly WriteJson _writeJson = new WriteJson();

        public Fassade(IToolRepository toolRepository, IOperationRepository operationRepository, IStationRepository stationRepository, IRepository<Line> lineRepository, IRepository<StationType> stationTypeRepository)
        {
            ToolRepository = toolRepository;
            OperationRepository = operationRepository;
            StationRepository = stationRepository;
            LineRepository = lineRepository;
            StationTypeRepository = stationTypeRepository;
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

        #region StationType
        public async Task<IEnumerable<StationType>> GetAllStationTypes()
        {
            return await StationTypeRepository.GetAll();
        }
        #endregion

        #region Station
        public async Task<IEnumerable<Station>> GetStationsFromLine(int lineID)
        {
            return await StationRepository.GetStationsFromLine(lineID);
        }

        public async Task UpdateStation(Station station)
        {
            station.lastModified = DateTime.Now;
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
            station.lastModified = DateTime.Now;
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

        public async Task<bool> StationExistsInLine(string name, int lineID)
        {
            var stations = await StationRepository.GetStationsFromLine(lineID);
            return stations.Any(station => station.assemblystation!.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
        #endregion

        #region Tool
        public async Task<IEnumerable<Tool>> GetToolsFromStation(int? stationID)
        {
            return await ToolRepository.GetToolsFromStation(stationID);
        }

        public async Task UpdateTool(Tool tool)
        {
            tool.lastModified = DateTime.Now;
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
            tool.lastModified = DateTime.Now;
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

        public async Task<bool> ToolExistsInStation(string name, int stationID)
        {
            var tools = await ToolRepository.GetToolsFromStation(stationID);
            return tools.Any(tool => tool.toolShortname!.Equals(name, StringComparison.OrdinalIgnoreCase));
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
        public async Task<IEnumerable<Operation>> GetOperationsFromTool(int? toolID)
        {
            return await OperationRepository.GetOperationsFromTool(toolID);
        }

        public async Task<IEnumerable<Operation>> GetAllOperations()
        {
            return await OperationRepository.GetAll();
        }

        public async Task UpdateOperation(Operation operation)
        {
            operation.lastModified = DateTime.Now;
            await OperationRepository.Update(operation);
            var tool = await ToolRepository.Get(operation.toolID);
            if (tool != null)
            {
                tool.lastModified = DateTime.Now;
                await ToolRepository.Update(tool);
            }
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

        public async Task AddOperation(Operation operation)
        {
            operation.lastModified = DateTime.Now;
            await OperationRepository.Add(operation);
            var tool = await ToolRepository.Get(operation.toolID);
            if (tool != null)
            {
                tool.lastModified = DateTime.Now;
                await ToolRepository.Update(tool);
            }
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

        public async Task<bool> OperationExistsByName(string name)
        {
            return await OperationRepository.ExistsByName(name);
        }

        public async Task<bool> OperationExistsInTool(string name, int toolID)
        {
            var operations = await OperationRepository.GetOperationsFromTool(toolID);
            return operations.Any(operation => operation.operationShortname!.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public async Task DeleteOperation(Operation operation)
        {
            await OperationRepository.Delete(operation);
            var tool = await ToolRepository.Get(operation.toolID);
            if (tool != null)
            {
                tool.lastModified = DateTime.Now;
                await ToolRepository.Update(tool);
            }
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

        #region Export
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
        #endregion

        #region Clone
        
        public T Clone<T>(ICopyable<T> obj)
        {
            return obj.Clone();
        }
        #endregion
    }
}

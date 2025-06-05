using DaimlerConfig.Components.Repositories;
using DaimlerConfig.Components.Models;
using DaimlerConfig.Components.JsonHandler;
using DaimlerConfig.Components.Export;
using DocumentFormat.OpenXml.Presentation;


namespace DaimlerConfig.Components.Fassade
{
    public class Fassade
    {
        public IToolRepository ToolRepository { get; private set; }
        public IRepository<ToolVersion> ToolVersionRepository { get; private set; }
        public IRepository<OperationVersion> OperationVersionRepository { get; private set; }
        public IOperationRepository OperationRepository { get; private set; }
        public IStationRepository StationRepository { get; private set; }
        public IRepository<Line> LineRepository { get; private set; }

        public IRepository<StationType> StationTypeRepository { get; private set; }

       
        public IRepository<ToolClass> ToolClassRepository { get; private set; }
        public IRepository<ToolType> ToolTypeRepository { get; private set; }

        public IRepository<DecisionClass> DecisionClassRepository { get; private set; }

        public IRepository<GenerationClass> GenerationClassRepository { get; private set; }

        public IRepository<SavingClass> SavingClassRepository { get; private set; }
        public IRepository<VerificationClass> VerificationClassRepository { get; private set; }
        
        public ExcelExport ExcelExport { get; private set; }
        
        private readonly WriteJson _writeJson = new WriteJson();

        public Fassade(IToolRepository toolRepository, IOperationRepository operationRepository, IStationRepository stationRepository, IRepository<Line> lineRepository, IRepository<StationType> stationTypeRepository, IRepository<DecisionClass> decisionClassRepository, IRepository<GenerationClass> generationClassRepository, IRepository<SavingClass> savingClassRepository, IRepository<VerificationClass> verificationClassRepository, IRepository<ToolClass> toolClassRepository, IRepository<ToolType> toolTypeRepository, ExcelExport ExcelExport, IRepository<ToolVersion> toolVersionRepository, IRepository<OperationVersion> operationVersionRepository)
        {
            ToolRepository = toolRepository;
            OperationRepository = operationRepository;
            StationRepository = stationRepository;
            LineRepository = lineRepository;
            StationTypeRepository = stationTypeRepository;
            DecisionClassRepository = decisionClassRepository;
            GenerationClassRepository = generationClassRepository;
            SavingClassRepository = savingClassRepository;
            VerificationClassRepository = verificationClassRepository;
            ToolClassRepository = toolClassRepository;
            ToolTypeRepository = toolTypeRepository;
            OperationVersionRepository = operationVersionRepository;
            this.ExcelExport = ExcelExport;
            ToolVersionRepository = toolVersionRepository;
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

        public async Task<bool> AddLine(Line line)
        {
            return await LineRepository.Add(line);
        }

        public async Task<bool> DeleteLine(Line line)
        {
            return await LineRepository.Delete(line);
        }

        public async Task<bool> LineExistsByName(string name)
        {
            return await LineRepository.ExistsByName(name);
        }

        public async Task<bool> UpdateLine(Line line)
        {
            return await LineRepository.Update(line);
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

        public async Task<Station> GetStation(int stationID)
        {
            return await StationRepository.Get(stationID);
        }

        public async Task<bool> UpdateStation(Station station)
        {
            station.lastModified = DateTime.Now;
            if (!await StationRepository.Update(station)) return false;

            if (await LineRepository.Get(station.lineID) is { } line)
            {
                line.lastModified = DateTime.Now;
                line.modifiedBy = station.modifiedBy;
                await LineRepository.Update(line);
            }

            return true;
        }


        public async Task<bool> AddStation(Station station)
        {
            station.lastModified = DateTime.Now;
            if (!await StationRepository.Add(station)) return false;

            if (await LineRepository.Get(station.lineID) is { } line)
            {
                line.lastModified = DateTime.Now;
                await LineRepository.Update(line);
            }

            return true;
        }


        public async Task<bool> StationExistsByName(string name)
        {
            return await StationRepository.ExistsByName(name);
        }

        public async Task<bool> DeleteStation(Station station)
        {
            if (!await StationRepository.Delete(station)) return false;

            if (await LineRepository.Get(station.lineID) is { } line)
            {
                line.lastModified = DateTime.Now;
                await LineRepository.Update(line);
            }

            return true;
        }


        public async Task<bool> StationExistsInLine(string name, int stationID, int lineID)
        {
            var stations = await StationRepository.GetStationsFromLine(lineID);

            return stations
                .Where(station => station.stationID != stationID)
                .Any(station => station.assemblystation!.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        #endregion

        #region Tool
        public async Task<IEnumerable<Tool>> GetToolsFromStation(int? stationID)
        {
            return await ToolRepository.GetToolsFromStation(stationID);
        }

        public async Task<bool> UpdateTool(Tool tool)
        {
            tool.lastModified = DateTime.Now;
            if (!await ToolRepository.Update(tool)) return false;

            var station = await StationRepository.Get(tool.stationID);
            if (station != null)
            {
                station.lastModified = DateTime.Now;
                station.modifiedBy = tool.modifiedBy;
                if (!await StationRepository.Update(station)) return false;

                var line = await LineRepository.Get(station.lineID);
                if (line != null)
                {
                    line.lastModified = DateTime.Now;
                    line.modifiedBy = tool.modifiedBy;
                    if (!await LineRepository.Update(line)) return false;
                }
            }

            return true;
        }

        public async Task<bool> UpdateToolWithVersion(Tool tool, Tool original)
        {
           
            var newVersion = ToolVersion.CreateToolVersionFromTool(original);

            
            var allVersions = await GetToolVersions(tool.toolID.Value);

            
            bool versionExists = allVersions.Any(v =>
                v.toolShortname == newVersion.toolShortname &&
                v.toolDescription == newVersion.toolDescription &&
                v.toolClassID == newVersion.toolClassID &&
                v.toolTypeID == newVersion.toolTypeID &&
                v.ipAddressDevice == newVersion.ipAddressDevice &&
                v.plcName == newVersion.plcName &&
                v.dbNoSend == newVersion.dbNoSend &&
                v.dbNoReceive == newVersion.dbNoReceive &&
                v.addressSendDB == newVersion.addressSendDB &&
                v.addressReceiveDB == newVersion.addressReceiveDB &&
                v.preCheckByte == newVersion.preCheckByte &&
                v.modifiedBy == newVersion.modifiedBy
           
            );

            if (!versionExists)
            {
                await ToolVersionRepository.Add(newVersion);
            }

            
            return await UpdateTool(tool);
        }


        public async Task<IEnumerable<ToolVersion>> GetToolVersions(int toolID)
        {
            return await ToolVersionRepository.Find(v => v.toolID == toolID);
        }





        public async Task<bool> AddTool(Tool tool)
        {
            tool.lastModified = DateTime.Now;
            if (!await ToolRepository.Add(tool)) return false;

            var station = await StationRepository.Get(tool.stationID);
            if (station != null)
            {
                station.lastModified = DateTime.Now;
                if (!await StationRepository.Update(station)) return false;

                var line = await LineRepository.Get(station.lineID);
                if (line != null)
                {
                    line.lastModified = DateTime.Now;
                    if (!await LineRepository.Update(line)) return false;
                }
            }

            return true;
        }


        public async Task<bool> ToolExistsByName(string name)
        {
            return await ToolRepository.ExistsByName(name);
        }

        public async Task<bool> ToolExistsInStation(string name, int toolID, int stationID)
        {
            var tools = await ToolRepository.GetToolsFromStation(stationID);

            return tools
                .Where(tool => tool.toolID != toolID) 
                .Any(tool => tool.toolShortname!.Equals(name, StringComparison.OrdinalIgnoreCase));
        }


        public async Task<bool> DeleteTool(Tool tool)
        {
            if (!await ToolRepository.Delete(tool)) return false;

            var station = await StationRepository.Get(tool.stationID);
            if (station != null)
            {
                station.lastModified = DateTime.Now;
                if (!await StationRepository.Update(station)) return false;

                var line = await LineRepository.Get(station.lineID);
                if (line != null)
                {
                    line.lastModified = DateTime.Now;
                    if (!await LineRepository.Update(line)) return false;
                }
            }

            return true;
        }


        public async Task<Tool> GetTool(int? toolID)
        {
            return await ToolRepository.Get(toolID);
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

        public async Task<bool> UpdateOperation(Operation operation)
        {
            operation.lastModified = DateTime.Now;
            if (!await OperationRepository.Update(operation)) return false;

            var tool = await ToolRepository.Get(operation.toolID);
            if (tool != null)
            {
                tool.lastModified = DateTime.Now;
                tool.modifiedBy = operation.modifiedBy;
                if (!await ToolRepository.Update(tool)) return false;

                var station = await StationRepository.Get(tool.stationID);
                if (station != null)
                {
                    station.lastModified = DateTime.Now;
                    station.modifiedBy = operation.modifiedBy;
                    if (!await StationRepository.Update(station)) return false;

                    var line = await LineRepository.Get(station.lineID);
                    if (line != null)
                    {
                        line.lastModified = DateTime.Now;
                        line.modifiedBy = operation.modifiedBy;
                        if (!await LineRepository.Update(line)) return false;
                    }
                }
            }

            return true;
        }

        public async Task<bool> UpdateOperationWithVersion(Operation operation, Operation original)
        {
            var version = OperationVersion.CreateOperationVersionFromOperation(original);
            await OperationVersionRepository.Add(version);


            return await UpdateOperation(operation);
        }

        public async Task<IEnumerable<OperationVersion>> GetOperationVersions(int operationID)
        {
            return await OperationVersionRepository.Find(v => v.operationID == operationID);
        }


        public async Task<bool> AddOperation(Operation operation)
        {
            operation.lastModified = DateTime.Now;
            if (!await OperationRepository.Add(operation)) return false;

            var tool = await ToolRepository.Get(operation.toolID);
            if (tool != null)
            {
                tool.lastModified = DateTime.Now;
                if (!await ToolRepository.Update(tool)) return false;

                var station = await StationRepository.Get(tool.stationID);
                if (station != null)
                {
                    station.lastModified = DateTime.Now;
                    if (!await StationRepository.Update(station)) return false;

                    var line = await LineRepository.Get(station.lineID);
                    if (line != null)
                    {
                        line.lastModified = DateTime.Now;
                        if (!await LineRepository.Update(line)) return false;
                    }
                }
            }

            return true;
        }


        public async Task<bool> OperationExistsByName(string name)
        {
            return await OperationRepository.ExistsByName(name);
        }

        public async Task<bool> OperationExistsInTool(string name, int operationID, int toolID)
        {
            var operations = await OperationRepository.GetOperationsFromTool(toolID);

            return operations
                .Where(operation => operation.operationID != operationID)
                .Any(operation => operation.operationShortname!.Equals(name, StringComparison.OrdinalIgnoreCase));
        }


        public async Task<bool> DeleteOperation(Operation operation)
        {
            if (!await OperationRepository.Delete(operation)) return false;

            var tool = await ToolRepository.Get(operation.toolID);
            if (tool != null)
            {
                tool.lastModified = DateTime.Now;
                if (!await ToolRepository.Update(tool)) return false;

                var station = await StationRepository.Get(tool.stationID);
                if (station != null)
                {
                    station.lastModified = DateTime.Now;
                    if (!await StationRepository.Update(station)) return false;

                    var line = await LineRepository.Get(station.lineID);
                    if (line != null)
                    {
                        line.lastModified = DateTime.Now;
                        if (!await LineRepository.Update(line)) return false;
                    }
                }
            }

            return true;
        }

        #endregion

        #region Export
        public async Task<string> Export(Line line)
        {
            var stations = await StationRepository.GetStationsFromLine(line.lineID);
            var tools = new List<Tool>();
            var operations = new List<Operation>();
            foreach (var station in stations)
            {
                var stationTools = await ToolRepository.GetToolsFromStation(station.stationID);
                tools.AddRange(stationTools);
            }
            foreach (var tool in tools)
            {
                var toolOperations = await OperationRepository.Find(o => o.toolID == tool.toolID);;
                operations.AddRange(toolOperations);
            }

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

        #region ToolClass/-Type
        public async Task<IEnumerable<ToolClass>> GetAllToolClasses()
        {
            return await ToolClassRepository.GetAll();
        }

        public async Task<IEnumerable<ToolType>> FindToolTypes(int toolClassID)
        {
            var result = await ToolTypeRepository.Find(t => t.toolClassID == toolClassID);
             return result.OrderBy(t => t.toolTypeName);

        }

        #endregion

        #region OperationClasses 

       

        public async Task<IEnumerable<DecisionClass>> GetDecisionClasses()
        {
            return await DecisionClassRepository.GetAll();
            
        }

        public async Task<IEnumerable<GenerationClass>> GetGenerationClasses(int templateID)
        {
            return await GenerationClassRepository.Find(t => t.TemplateId == templateID);
            
        }

        public async Task<IEnumerable<SavingClass>> GetSavingClasses(int templateID)
        {
            return await SavingClassRepository.Find(t => t.TemplateId == templateID);

        }

        public async Task<IEnumerable<VerificationClass>> GetVerificationClasses(int templateID)
        {
            return await VerificationClassRepository.Find(t => t.TemplateId == templateID);

        }

        public async Task<VerificationClass> GetVerificationClass(int id)
        {
            return await VerificationClassRepository.Get(id);
        }



        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DaimlerConfig.Components.Repositories;
using Microsoft.AspNetCore.Components;
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
        #endregion 
    }



}

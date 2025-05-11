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

        public async Task<IEnumerable<Line>> GetAllLinesAsync()
        {
            return await LineRepository.GetAll();
        }

    }



}

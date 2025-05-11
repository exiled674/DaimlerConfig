using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DaimlerConfig.Components.Repositories;

namespace DaimlerConfig.Components.Fassade
{
    public class Fassade
    {
        public readonly IToolRepository ToolRepository;
        public readonly IOperationRepository OperationRepository;
        public readonly IStationRepository StationRepository;

        public Fassade(IToolRepository toolRepository, IOperationRepository operationRepository, IStationRepository stationRepository)
        {
            ToolRepository = toolRepository;
            OperationRepository = operationRepository;
            StationRepository = stationRepository;
        }
    }
}

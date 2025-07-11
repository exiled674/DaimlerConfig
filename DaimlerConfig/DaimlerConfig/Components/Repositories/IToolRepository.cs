﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DaimlerConfig.Components.Models;

namespace DaimlerConfig.Components.Repositories
{
    public interface IToolRepository : IRepository<Tool>
    {
        Task<IEnumerable<Tool>> GetToolsFromStation(int? stationID);


    }
}

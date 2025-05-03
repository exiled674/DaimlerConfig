using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaimlerConfig.Components.Models
{
    public class Station
    {
        private int stationID {get; set;}

        private string? assemblyStation {get; set;}

        private string? stationName {get; set;}

        private int stationType { get; set; }

        private DateTime? lastModified {get; set;}
    }

 
}

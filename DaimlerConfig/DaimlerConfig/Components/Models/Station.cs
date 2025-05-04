using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaimlerConfig.Components.Models
{
    public class Station
    {
        public int stationID {get; set;}

        public string? assemblystation {get; set;}

        public string? stationName {get; set;}

        public int StationType_stationTypeID { get; set; }

        public DateTime? lastModified {get; set;}
    }

 
}

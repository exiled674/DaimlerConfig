using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaimlerConfig.Components.Models
{
    public class Station
    {

        public int stationID {get; set;}

        [MaxLength(10, ErrorMessage = "Maximum of 10 characters for assemblystation allowed!")]
        public string? assemblystation {get; set;}

        [MaxLength(50, ErrorMessage = "Maximum of 10 characters for assemblystation allowed!")]
        public string? stationName {get; set;}

        [Required(ErrorMessage = "A station type must be specified!")]
        public int StationType_stationTypeID { get; set; }

        public DateTime? lastModified {get; set;}
    }

 
}

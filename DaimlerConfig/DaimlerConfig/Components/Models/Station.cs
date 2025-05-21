using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaimlerConfig.Components.Models
{
    public class Station : ICopyable<Station>, IEquatable<Station>
    {
       
    }

    public int? stationID { get; set; } = 0;

        public int? lineID { get; set; }

        public string? assemblystation { get; set; }

        public string? stationName { get; set; }

        public int stationTypeID { get; set; }

        public DateTime? lastModified { get; set; }

        public Station Clone()
        {
            var clone = (Station)this.MemberwiseClone();
            clone.stationID = 0;
            clone.lineID = null;
            return clone;
        }

        public bool Equals(Station? other)
        {
            if (other == null)
                return false;

            return stationID == other.stationID
                && lineID == other.lineID
                && string.Equals(assemblystation, other.assemblystation, StringComparison.OrdinalIgnoreCase)
                && string.Equals(stationName, other.stationName, StringComparison.OrdinalIgnoreCase)
                && stationTypeID == other.stationTypeID;
        }
    }


}

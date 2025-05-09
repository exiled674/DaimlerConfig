using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaimlerConfig.Components.Models
{
    public class Line
    {
        public int lineID { get; set; }

        public string? lineName { get; set; }

       public DateTime? lastModified { get; set; }
    }
}

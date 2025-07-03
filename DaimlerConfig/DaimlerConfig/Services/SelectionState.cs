using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DConfig.Services
{
    public class SelectionState
    {
        public int? StationID { get; set; }
        public int? ToolID { get; set; }
        public int? OperationID { get; set; }
        public DateTime Timestamp { get; set; }

    }
}

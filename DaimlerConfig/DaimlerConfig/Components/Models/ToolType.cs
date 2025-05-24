using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaimlerConfig.Components.Models
{
    public class ToolType
    {
        public int toolTypeID { get; init; }
        public string toolTypeName { get; init; }
        public int toolClassID { get; init; }
        public string HelpText { get; init; }
    }
}

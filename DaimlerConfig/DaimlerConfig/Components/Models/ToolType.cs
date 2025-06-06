using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaimlerConfig.Components.Models
{
    public class ToolType
    {
        public int toolTypeID { get; set; }
        public string toolTypeName { get; set; }
        public int toolClassID { get; set; }
        public string HelpText { get; set; }
    }
}

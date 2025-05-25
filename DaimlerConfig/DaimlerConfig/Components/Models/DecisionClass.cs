using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaimlerConfig.Components.Models
{
    public class DecisionClass 
    {
        public int decisionClassID { get; init; }
        public string decisionClassName { get; init; }
        public int TemplateId { get; init; }
        public string HelpText { get; init; }
    }
}

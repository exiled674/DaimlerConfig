using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DConfig.Components.Models
{
    public class GenerationClass 
    {
        public int generationClassID { get; init; }
        public string generationClassName { get; init; }
        public int TemplateId { get; init; }
        public string HelpText { get; init; }
    }
}

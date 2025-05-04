using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaimlerConfig.Components.Models
{
    public class Operation
    {
        public int OperationID { get; set; }
        public string OperationShortname { get; set; }
        public string OperationDescription { get; set; }
        public string OperationSequence { get; set; }
        public string OperationSequenceGroup { get; set; }
        public string OperationDecisionCriteria { get; set; }
        public bool AlwaysPerform { get; set; }
        public int DecisionClass { get; set; }
        public int GenerationClass { get; set; }
        public int VerificationClass { get; set; }
        public int SavingClass { get; set; }
        public int ToolID { get; set; }
        public bool ParallelSerial { get; set; } // z. B. "parallel" oder "serial"
        public bool QGateRelevant { get; set; }
        public DateTime LastModified { get; set; }
    }


}

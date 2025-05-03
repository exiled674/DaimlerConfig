using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaimlerConfig.Components.Models
{
    public class Operation
    {
        private int OperationID { get; set; }
        private string OperationShortname { get; set; }
        private string OperationDescription { get; set; }
        private string OperationSequence { get; set; }
        private string OperationSequenceGroup { get; set; }
        private string OperationDecisionCriteria { get; set; }
        private bool AlwaysPerform { get; set; }
        private int DecisionClass { get; set; }
        private int GenerationClass { get; set; }
        private int VerificationClass { get; set; }
        private int SavingClass { get; set; }
        private int ToolID { get; set; }
        public bool ParallelSerial { get; set; } // z. B. "parallel" oder "serial"
        private bool QGateRelevant { get; set; }
        private DateTime LastModified { get; set; }
    }


}

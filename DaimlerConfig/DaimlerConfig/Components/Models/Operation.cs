using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaimlerConfig.Components.Models
{
    public class Operation
    {
        public int OperationID { get; set; }

        public int ToolID { get; set; }



        [MaxLength(16, ErrorMessage = "Maximum of 10 characters for operation shortname allowed!")]
        public string? OperationShortname { get; set; }

        [MaxLength(100, ErrorMessage = "Maximum of 10 characters for operation description allowed!")]
        public string? OperationDescription { get; set; }

        // Parallel wäre 1P1, Seriell 1.1 TODO
        public string? OperationSequence { get; set; }


        [MaxLength(100, ErrorMessage = "Maximum of 10 characters for operation sequence group allowed!")]
        public string? OperationSequenceGroup { get; set; }

        // Ka noch
        public string? OperationDecisionCriteria { get; set; }


        public bool AlwaysPerform { get; set; }


        public int DecisionClass { get; set; }
        public int GenerationClass { get; set; }
        public int VerificationClass { get; set; }
        public int SavingClass { get; set; }

        public bool ParallelSerial { get; set; } 
        public bool QGateRelevant { get; set; }


        public DateTime LastModified { get; set; }
    }


}

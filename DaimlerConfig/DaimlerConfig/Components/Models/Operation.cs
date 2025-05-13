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
        public int operationID { get; set; }
        public required string? operationShortname { get; set; }
        public required string? operationDescription { get; set; }
        public required string? operationSequence { get; set; }
        public required string? operationSequenceGroup { get; set; }
        public required string? operationDecisionCriteria { get; set; }
        public bool alwaysPerform { get; set; }
        public int decisionClassID { get; set; }
        public int generationClassID { get; set; }
        public int verificationClassID { get; set; }
        public int savingClassID { get; set; }
        public int toolID { get; set; }
        public bool parallel { get; set; }
        public int qGateID { get; set; }
        public DateTime lastModified { get; set; }
    }


}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaimlerConfig.Components.Models
{
    public class Operation : ICopyable<Operation>, IEquatable<Operation>
    {
        public int? operationID { get; set; } = 0;

        public int? toolID { get; set; }
        
        public  string? operationShortname { get; set; }
        public  string? operationDescription { get; set; }
        public  string? operationSequence { get; set; }
        public  string? operationSequenceGroup { get; set; }
        public  string? operationDecisionCriteria { get; set; }
        public bool alwaysPerform { get; set; }
        public int decisionClassID { get; set; }
        public int generationClassID { get; set; }
        public int verificationClassID { get; set; }
        public int savingClassID { get; set; }
       
        public bool parallel { get; set; }
        public int qGateID { get; set; } = 1;
        public DateTime? lastModified { get; set; }

        public bool? isLocked { get; set; }

        public string? lockedBy { get; set; }

        public DateTime? lockTimestamp { get; set; }
        
        public string Comment { get; set; } = "";
        
        public Status Status { get; set; } = Status.Undefined;
        
        public Operation Clone()
        {
            var clone = (Operation)this.MemberwiseClone();
            clone.operationID = 0;
            clone.toolID = null;
            return clone;
        }

        public bool Equals(Operation? other)
        {
            if (other == null) return false;

            return operationID == other.operationID
                && toolID == other.toolID
                && string.Equals(operationShortname, other.operationShortname, StringComparison.Ordinal)
                && string.Equals(operationDescription, other.operationDescription, StringComparison.Ordinal)
                && string.Equals(operationSequence, other.operationSequence, StringComparison.Ordinal)
                && string.Equals(operationSequenceGroup, other.operationSequenceGroup, StringComparison.Ordinal)
                && string.Equals(operationDecisionCriteria, other.operationDecisionCriteria, StringComparison.Ordinal)
                && alwaysPerform == other.alwaysPerform
                && decisionClassID == other.decisionClassID
                && generationClassID == other.generationClassID
                && verificationClassID == other.verificationClassID
                && savingClassID == other.savingClassID
                && parallel == other.parallel
                && qGateID == other.qGateID;
        }

    }


}
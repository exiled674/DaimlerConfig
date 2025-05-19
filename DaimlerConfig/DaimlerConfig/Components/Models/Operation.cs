using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaimlerConfig.Components.Models
{
    public class Operation : ICopyable<Operation>
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
        public int qGateID { get; set; }
        public DateTime? lastModified { get; set; }

        public Operation Clone()
        {
            var clone = (Operation)this.MemberwiseClone();
            clone.operationID = 0;
            clone.toolID = null;
            return clone;
        }
    }


}
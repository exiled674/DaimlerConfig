using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaimlerConfig.Components.Models
{
    public class OperationVersion : Operation
    {
        public int? operationVersionID { get; set; }

        public static OperationVersion CreateOperationVersionFromOperation(Operation operation)
        {
            return new OperationVersion
            {
                // operationVersionID nicht setzen, ist Identity/Auto Increment

                operationID = operation.operationID,
                toolID = operation.toolID,
                operationShortname = operation.operationShortname,
                operationDescription = operation.operationDescription,
                operationSequence = operation.operationSequence,
                operationSequenceGroup = operation.operationSequenceGroup,
                operationDecisionCriteria = operation.operationDecisionCriteria,
                alwaysPerform = operation.alwaysPerform,
                decisionClassID = operation.decisionClassID,
                generationClassID = operation.generationClassID,
                verificationClassID = operation.verificationClassID,
                savingClassID = operation.savingClassID,
                parallel = operation.parallel,
                qGateID = operation.qGateID,
                lastModified = operation.lastModified,

               
            };
        }
    }
}
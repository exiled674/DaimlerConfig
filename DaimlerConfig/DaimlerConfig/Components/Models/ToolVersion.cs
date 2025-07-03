
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DConfig.Components.Models
{
    public class ToolVersion : Tool
    {
        public int? toolVersionID { get; set; }

        public static ToolVersion CreateToolVersionFromTool(Tool tool)
        {
            return new ToolVersion
            {
                // toolVersionID nicht setzen, ist Identity/Auto Increment

                toolID = tool.toolID,
                stationID = tool.stationID,
                toolShortname = tool.toolShortname,
                toolDescription = tool.toolDescription,
                toolClassID = tool.toolClassID,
                toolTypeID = tool.toolTypeID,
                ipAddressDevice = tool.ipAddressDevice,
                plcName = tool.plcName,
                dbNoSend = tool.dbNoSend,
                dbNoReceive = tool.dbNoReceive,
                preCheckByte = tool.preCheckByte,
                addressSendDB = tool.addressSendDB,
                addressReceiveDB = tool.addressReceiveDB,
                isLocked = tool.isLocked,
                lockedBy = tool.lockedBy,
                lastModified = tool.lastModified,
                lockTimestamp = tool.lockTimestamp,
                modifiedBy = tool.modifiedBy
            };
        }
    }

}
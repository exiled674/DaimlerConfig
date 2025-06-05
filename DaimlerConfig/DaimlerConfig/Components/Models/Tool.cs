using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DaimlerConfig.Components.Models
{
    public class Tool : ICopyable<Tool>, IEquatable<Tool>
    {
        public int? toolID { get; set; } = 0;

        public int? stationID { get; set; }

        public string? toolShortname { get; set; }

        public string? toolDescription { get; set; }

        public int? toolClassID { get; set; }

        public int? toolTypeID { get; set; }

        public string? ipAddressDevice { get; set; } = "0.0.0.0";

        public string? plcName { get; set; }

        public string? dbNoSend { get; set; }
        public string? dbNoReceive { get; set; }

        public int preCheckByte { get; set; }

        public string? addressSendDB { get; set; }

        public string? addressReceiveDB { get; set; }

        public DateTime? lastModified { get; set; }

        public string? modifiedBy { get; set; }

        public bool? isLocked {  get; set; }

        public string? lockedBy { get; set; }

        public DateTime? lockTimestamp { get; set; }


        public Tool Clone()
        {
            var clone = (Tool)this.MemberwiseClone();
            clone.toolID = 0;
            clone.stationID = null;
            return clone;
        }

        public bool Equals(Tool? other)
        {
            if (other == null) return false;

            return toolID == other.toolID
                && stationID == other.stationID
                && string.Equals(toolShortname, other.toolShortname, StringComparison.Ordinal)
                && string.Equals(toolDescription, other.toolDescription, StringComparison.Ordinal)
                && toolTypeID == other.toolTypeID
                && string.Equals(ipAddressDevice, other.ipAddressDevice, StringComparison.Ordinal)
                && string.Equals(plcName, other.plcName, StringComparison.Ordinal)
                && string.Equals(dbNoSend, other.dbNoSend, StringComparison.Ordinal)
                && string.Equals(dbNoReceive, other.dbNoReceive, StringComparison.Ordinal)
                && preCheckByte == other.preCheckByte
                && string.Equals(addressSendDB, other.addressSendDB, StringComparison.Ordinal)
                && string.Equals(addressReceiveDB, other.addressReceiveDB, StringComparison.Ordinal);
        }


    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DaimlerConfig.Components.Models
{
    public class Tool
    {
        public int toolID { get; set; }

        public int stationID { get; set; }

        public string? toolShortname { get; set; }

        public string? toolDescription { get; set; }

        public int toolTypeID { get; set; }

        public string? ipAddressDevice { get; set; }

        public string? plcName { get; set; }

        public string? dbNoSend { get; set; }
        public string? dbNoReceive { get; set; }

        public int preCheckByte { get; set; }

        public string? addressSendDB { get; set; }

        public string? addressReceiveDB { get; set; }

        public DateTime? lastModified { get; set; }


    }
}

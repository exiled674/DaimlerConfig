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
        public int toolID {  get; set; }


        public int stationID {  get; set; }


        [MaxLength(16, ErrorMessage = "Maximum of 10 characters for tool shortname allowed!")]
        public string? toolShortname { get; set; }


        [MaxLength(100, ErrorMessage = "Maximum of 10 characters for tool description allowed!")]
        public string? toolDescription { get; set; }


        public int toolClassID { get; set; }


        public int toolTypeID { get; set; }


        [RegularExpression(@"^(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)(\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)){3}$",
        ErrorMessage = "Invalid IP address format.")]
        public string? ip_adress {  get; set; }


        [MaxLength(50, ErrorMessage = "Maximum of 10 characters for tool PLC-Name allowed!")]
        public string? plcName { get; set; }


        [MaxLength(50, ErrorMessage = "Maximum of 10 characters for DBNo Send allowed!")]
        public string? dbNoSend { get; set; }


        [MaxLength(50, ErrorMessage = "Maximum of 10 characters for DBNo Receive allowed!")]
        public string? dbNoReceive { get; set; }


        public int preCheckByte { get; set; }

        public string? adressSendDB { get; set; }

        public string? adressReceiveDB { get; set; }

        public DateTime? lastModified {  get; set; }


    }
}

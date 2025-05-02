using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DaimlerConfig.Components.Models
{
    public class Tool
    {
        private int toolID {  get; set; }

        private int stationID {  get; set; }

        private string? toolShortname { get; set; }

        private string? toolDescription { get; set; }

        private string? toolClass { get; set; }

        private string? toolType { get; set; }

        private string? ip_adress {  get; set; }

        private string? plcName { get; set; }

        private string? dbNoSend { get; set; }
        private string? dbNoReceive { get; set; }

        private string? preCheckByte { get; set; }

        private string? adressSendDB { get; set; }

        private string? adressReceiveDB { get; set; }

        private DateTime? lastModified {  get; set; }


    }
}

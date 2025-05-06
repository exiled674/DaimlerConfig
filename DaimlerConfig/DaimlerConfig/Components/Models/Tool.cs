namespace DaimlerConfig.Components.Models
{
    public class Tool
    {
        public int ToolId {  get; set; }

        public int StationId {  get; set; }

        public string? ToolShortname { get; set; }

        public string? ToolDescription { get; set; }

        public int ToolClassId { get; set; }

        public int ToolTypeId { get; set; }

        public string? IpAddress {  get; set; }

        public string? PlcName { get; set; }

        public string? DbNoSend { get; set; }
        public string? DbNoReceive { get; set; }

        public int PreCheckByte { get; set; }

        public string? AddressSendDb { get; set; }

        public string? AddressReceiveDb { get; set; }

        public DateTime? LastModified {  get; set; }


    }
}

namespace DaimlerConfig.Components.Models
{
    public class Tool
    {
        public int toolID {  get; set; }

        public int stationID {  get; set; }

        public string? toolShortname { get; set; }

        public string? toolDescription { get; set; }

        public int toolClassID { get; set; }

        public int toolTypeID { get; set; }

        public string? IPAdress {  get; set; }

        public string? PLCName { get; set; }

        public string? DB_NoSend { get; set; }
        public string? DB_NoReceive { get; set; }

        public int preCheck_Byte { get; set; }

        public string? adress_sendDB { get; set; }

        public string? adress_receiveDB { get; set; }
        
        public DateTime? lastModified {  get; set; }


    }
}

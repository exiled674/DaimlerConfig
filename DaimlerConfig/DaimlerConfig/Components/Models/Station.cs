namespace DaimlerConfig.Components.Models
{
    public class Station
    {
        public int stationID {get; set;}

        public string? assemblystation {get; set;}

        public string? stationName {get; set;}

        public int StationType_stationTypeID { get; set; }
        
        public DateTime? lastModified {get; set;}
    }

 
}

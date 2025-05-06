namespace DaimlerConfig.Components.Models
{
    public class Station
    {
        public int StationId {get; set;}

        public string? Assemblystation {get; set;}

        public string? StationName {get; set;}

        public int StationType_stationTypeID { get; set; }

        public DateTime? LastModified {get; set;}
    }

 
}

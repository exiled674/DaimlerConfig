using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DaimlerConfig.Components.Util;

public class WriteJson
{
    private readonly JsonSerializerOptions _options;

    public WriteJson()
    {
        _options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            ReferenceHandler = ReferenceHandler.Preserve
        };
    }

    /// <summary>
    /// Konvertiert eine Liste von allen Elementen in einen JSON-String
    /// </summary>
    public async Task<string> WriteAllToFileAsync(
    List<Models.Station> stations,
    List<Models.Tool> tools,
    List<Models.Operation> operations
       )
    {
        var allData = new
        {
            Stations = stations,
            Tools = tools,
            Operations = operations
        };

        return JsonSerializer.Serialize(allData, _options);
    }


    /// <summary>
    /// Konvertiert eine Liste von StationTypes in einen JSON-String
    /// </summary>
    public string SerializeStationTypes(List<Models.StationType> stationTypes)
    {
        return JsonSerializer.Serialize(stationTypes, _options);
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DaimlerConfig.Components.JsonHandler;

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
        public async Task WriteAllToFileAsync(
            List<Models.Station> stations,
            List<Models.Tool> tools,
            List<Models.Operation> operations,
            string filePath)
        {
            var allData = new
            {
                Stations = stations,
                Tools = tools,
                Operations = operations
            };

            try
            {
                await using var fs = File.Create(filePath);
                await JsonSerializer.SerializeAsync(fs, allData, _options);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Schreiben der Datei: {ex.Message}");
                throw;
            }
        }

        
        /// <summary>
        /// Konvertiert eine Liste von StationTypes in einen JSON-String
        /// </summary>
        public string SerializeStationTypes(List<Models.StationType> stationTypes)
        {
            return JsonSerializer.Serialize(stationTypes, _options);
        }
    }
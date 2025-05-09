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
        /// Konvertiert eine Liste von Stationen in einen JSON-String
        /// </summary>
        public string SerializeStations(List<Models.Station> stations, List<Models.Tool> tools, List<Models.Operation> operations)
        {
            // Liste mit Stationen, in die Tools (inkl. Operations) eingebettet werden
            var enrichedStations = new List<Models.Station>();

            foreach (var station in stations)
            {
                // Tools mit zugehörigen Operations einbetten
                var relatedTools = SerializeTools(station.stationID, tools, operations);
                station.Tools = relatedTools;
                enrichedStations.Add(station);
            }

            return JsonSerializer.Serialize(enrichedStations, _options);
        }

        public List<Models.Tool> SerializeTools(int stationId, List<Models.Tool> tools, List<Models.Operation> operations)
        {
            var result = new List<Models.Tool>();

            foreach (var tool in tools)
            {
                if (tool.stationID == stationId)
                {
                    // Operations einbetten
                    var relatedOps = SerializeOperations(tool.toolID, operations);
                    tool.Operations = relatedOps;
                    result.Add(tool);
                }
            }

            return result;
        }

        public List<Models.Operation> SerializeOperations(int toolId, List<Models.Operation> operations)
        {
            return operations.FindAll(op => op.ToolId == toolId);
        }


        /// <summary>
        /// Konvertiert eine Liste von StationTypes in einen JSON-String
        /// </summary>
        public string SerializeStationTypes(List<Models.StationType> stationTypes)
        {
            return JsonSerializer.Serialize(stationTypes, _options);
        }

        /// <summary>
        /// Schreibt eine Liste von Stationen in eine JSON-Datei
        /// </summary>
        public async Task WriteStationsToFileAsync(List<Models.Station> stations, string filePath)
        {
            try
            {
                using FileStream fs = File.Create(filePath);
                await JsonSerializer.SerializeAsync(fs, stations, _options);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Schreiben der Datei: {ex.Message}");
                throw;
            }
        }
    }
using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using DaimlerConfig.Components.Infrastructure;
using DaimlerConfig.Components.Models;
using DaimlerConfig.Components.Repositories;

namespace DaimlerConfig
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();
#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif
            builder.Services.AddSingleton<IDbConnectionFactory>(sp =>
                new SqliteConnectionFactory(Path.Combine(Directory.GetCurrentDirectory(), "meineDatenbank.db")));
            builder.Services.AddSingleton<DatabaseInitializer>();
            builder.Services.AddScoped<IRepository<Station>, Repository<Station>>();
            builder.Services.AddScoped<IRepository<StationType>, Repository<StationType>>();
            builder.Services.AddScoped<IRepository<Line>, Repository<Line>>();
            builder.Services.AddScoped<IRepository<Tool>, Repository<Tool>>();

            var app = builder.Build();

            app.Services.GetRequiredService<DatabaseInitializer>().EnsureCreated();

            var lineRepo = app.Services.GetRequiredService<IRepository<Line>>();
            var stationTypeRepo = app.Services.GetRequiredService<IRepository<StationType>>();
            var stationRepo = app.Services.GetRequiredService<IRepository<Station>>();
            var toolRepo = app.Services.GetRequiredService<IRepository<Tool>>();



            var lines = new[]
    {
         new Line { lineName = "Line1", lastModified = DateTime.Now },
       new Line { lineName = "Line2", lastModified = DateTime.Now },
       new Line { lineName = "Line3", lastModified = DateTime.Now }
    };

            foreach (var line in lines)
            {
                lineRepo.Add(line);
            }

            // Schritt 1: Stationstypen erstellen
            var stationTypes = new[]
            {
        new StationType { stationTypeName = "Assembly" },
        new StationType { stationTypeName = "Testing" },
        new StationType { stationTypeName = "Packaging" }
    };

            foreach (var stationType in stationTypes)
            {
                stationTypeRepo.Add(stationType);
            }

            // Schritt 2: Stationen erstellen
            var stations = Enumerable.Range(1, 10).Select(i => new Station
            {
                assemblystation = $"Station{i}",
                stationName = $"StationName{i}",
                StationType_stationTypeID = 1, // Zyklisch Stationstypen zuweisen
                lineID = 1,
                lastModified = DateTime.Now
            }).ToList();

            foreach (var station in stations)
            {
                stationRepo.Add(station);
            }

            // Schritt 3: Tools erstellen
            var tools = Enumerable.Range(1, 5).Select(i => new Tool
            {
                toolShortname = $"Tool{i}",
                toolDescription = $"Description for Tool{i}",
                stationID = stations[i % 10].stationID, // Zyklisch Stationen zuweisen
                toolClassID = 1, // Beispielwert
                toolTypeID = 1, // Beispielwert
                ip_adress = $"192.168.0.{i}",
                plcName = $"PLC{i}",
                dbNoSend = $"DB{i}Send",
                dbNoReceive = $"DB{i}Receive",
                preCheckByte = 0,
                adressSendDB = $"AddressSend{i}",
                adressReceiveDB = $"AddressReceive{i}",
                lastModified = DateTime.Now
            }).ToList();

            foreach (var tool in tools)
            {
                toolRepo.Add(tool);
            }

            return app;
        }
    }
}

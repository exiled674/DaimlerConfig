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

            var app = builder.Build();

            app.Services.GetRequiredService<DatabaseInitializer>().EnsureCreated();

            var stationTypeRepo = app.Services.GetRequiredService<IRepository<StationType>>();
            var existingTypes = stationTypeRepo.GetAll().GetAwaiter().GetResult();
            if (!existingTypes.Any())
            {
                stationTypeRepo.Add(new StationType
                {
                    stationTypeName = "Default"
                }).GetAwaiter().GetResult();
                existingTypes = stationTypeRepo.GetAll().GetAwaiter().GetResult();
            }

            var stationRepo = app.Services.GetRequiredService<IRepository<Station>>();
            
           
            
                stationRepo.Add(new Station
                {
                    assemblystation = "A1",
                    stationName = "Eingang",
                    StationType_stationTypeID = existingTypes.First().stationTypeID,
                    lastModified = DateTime.Now
                }).GetAwaiter().GetResult();

                stationRepo.Add(new Station
                {
                    assemblystation = "A2",
                    stationName = "Eingan2",
                    StationType_stationTypeID = existingTypes.First().stationTypeID,
                    lastModified = DateTime.Now
                }).GetAwaiter().GetResult();

            var existingStations = stationRepo.GetAll().GetAwaiter().GetResult();
            

            var toDelete = existingStations.FirstOrDefault(s => s.assemblystation == "A1" && s.stationName == "Eingang");
            if (toDelete != null)
            {
                stationRepo.Delete(toDelete).GetAwaiter().GetResult();
            }

            return app;
        }
    }
}

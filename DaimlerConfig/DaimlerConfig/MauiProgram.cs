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
            // Connection‑Factory und DB‑Initializer
            builder.Services.AddSingleton<IDbConnectionFactory>(sp =>
                new SqliteConnectionFactory(
                    Path.Combine(Directory.GetCurrentDirectory(), "meineDatenbank.db")
                )
            );
            builder.Services.AddSingleton<DatabaseInitializer>();

            // Repositorys registrieren
            builder.Services.AddScoped<IRepository<Station>, Repository<Station>>();
            builder.Services.AddScoped<IRepository<StationType>, Repository<StationType>>();

            var app = builder.Build();

            // 1) Schema anlegen
            app.Services
               .GetRequiredService<DatabaseInitializer>()
               .EnsureCreated();

            // 2) StationType seeden (hartkodiert)
            var stationTypeRepo = app.Services.GetRequiredService<IRepository<StationType>>();
            var existingTypes = stationTypeRepo.GetAll().GetAwaiter().GetResult();
            if (!existingTypes.Any())
            {
                stationTypeRepo.Add(new StationType
                {
                    // stationTypeID AUTOINCREMENT => nicht setzen
                    stationTypeName = "Default"
                }).GetAwaiter().GetResult();
            }

            // 3) Station seeden
            var stationRepo = app.Services.GetRequiredService<IRepository<Station>>();
            var existingStations = stationRepo.GetAll().GetAwaiter().GetResult();
            if (!existingStations.Any())
            {
                stationRepo.Add(new Station
                {
                    assemblystation = "A1",
                    stationName = "Eingang",
                    StationType_stationTypeID = existingTypes.FirstOrDefault()?.stationTypeID ?? 0,
                    lastModified = DateTime.Now
                }).GetAwaiter().GetResult();
            }

            return app;
        }
    }
}

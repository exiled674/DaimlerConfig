using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using DaimlerConfig.Components.Infrastructure;
using DaimlerConfig.Components.Models;
using DaimlerConfig.Components.Repositories;
using DaimlerConfig.Components.Fassade;
using DaimlerConfig.Services;
using Microsoft.AspNetCore.SignalR.Client;
using System.Text.Json;

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

            // 1. Konfigurationsdatei einbinden
            string benutzerOrdner = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string dateiPfad = Path.Combine(benutzerOrdner, "appsettings.json");
            builder.Configuration
                   .AddJsonFile(dateiPfad, optional: false, reloadOnChange: true);

            builder.Services.AddMauiBlazorWebView();
#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif      
            builder.Services.AddSingleton<SignalRService>();
            builder.Services.AddSingleton<DirtyManagerService>();


            // 2. Azure SQL Server ConnectionFactory registrieren
            var sqlConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddSingleton<IDbConnectionFactory>(sp =>
                new SqlServerConnectionFactory(sqlConnectionString));

            // 3. Initializer und Repositories
           
            builder.Services.AddSingleton<IToolRepository, ToolRepository>();
            builder.Services.AddSingleton<IOperationRepository, OperationRepository>();
            builder.Services.AddSingleton<IStationRepository, StationRepository>();
            builder.Services.AddScoped<IRepository<Line>, Repository<Line>>();
            builder.Services.AddScoped<IRepository<StationType>, Repository<StationType>>();

            builder.Services.AddSingleton<Fassade>(sp =>
            {
                
                var toolRepo = sp.GetRequiredService<IToolRepository>();
                var operationRepo = sp.GetRequiredService<IOperationRepository>();
                var stationRepo = sp.GetRequiredService<IStationRepository>();
                var lineRepo = sp.GetRequiredService<IRepository<Line>>();
                var stationType = sp.GetRequiredService<IRepository<StationType>>();

                return new Fassade(toolRepo, operationRepo, stationRepo, lineRepo, stationType);
            });


            //SignalR
            var configPfad = Path.Combine(benutzerOrdner, "signalRVPS.json");
            string hubURL = null;

            // JSON-Datei einlesen und SignalR-URL extrahieren
            if (File.Exists(configPfad))
            {
                var jsonInhalt = File.ReadAllText(configPfad);
                using var jsonDoc = JsonDocument.Parse(jsonInhalt);
                if (jsonDoc.RootElement.TryGetProperty("SignalRHubUrl", out var urlElement))
                {
                    hubURL = urlElement.GetString();
                }
            }

            if (string.IsNullOrWhiteSpace(hubURL))
            {
                throw new Exception("SignalR-URL konnte nicht aus der Konfiguration geladen werden.");
            }

            var connection = new HubConnectionBuilder().WithUrl(hubURL).Build();
            builder.Services.AddSingleton(connection);

            var app = builder.Build();


            
            

            return app;
        }
    }
}

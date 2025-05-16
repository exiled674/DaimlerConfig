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

            // 2. Azure SQL Server ConnectionFactory registrieren
            var sqlConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddSingleton<IDbConnectionFactory>(sp =>
                new SqlServerConnectionFactory(sqlConnectionString));

            // 3. Initializer und Repositories
            builder.Services.AddSingleton<DatabaseInitializer>();
            builder.Services.AddSingleton<IToolRepository, ToolRepository>();
            builder.Services.AddSingleton<IOperationRepository, OperationRepository>();
            builder.Services.AddSingleton<IStationRepository, StationRepository>();
            builder.Services.AddScoped<IRepository<Line>, Repository<Line>>();

            builder.Services.AddSingleton<Fassade>(sp =>
            {
                var toolRepo = sp.GetRequiredService<IToolRepository>();
                var operationRepo = sp.GetRequiredService<IOperationRepository>();
                var stationRepo = sp.GetRequiredService<IStationRepository>();
                var lineRepo = sp.GetRequiredService<IRepository<Line>>();

                return new Fassade(toolRepo, operationRepo, stationRepo, lineRepo);
            });

            var app = builder.Build();


            // 4. Datenbank sicherstellen (für EF Core oder eigene Implementierung)
            app.Services.GetRequiredService<DatabaseInitializer>().EnsureCreated();

            return app;
        }
    }
}

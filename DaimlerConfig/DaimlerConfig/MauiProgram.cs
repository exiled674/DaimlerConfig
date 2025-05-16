using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using DaimlerConfig.Components.Infrastructure;
using DaimlerConfig.Components.Models;
using DaimlerConfig.Components.Repositories;
using DaimlerConfig.Components.Fassade;
using CommunityToolkit.Maui;

namespace DaimlerConfig
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder.UseMauiApp<App>().ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            }).UseMauiCommunityToolkit();
            builder.Services.AddMauiBlazorWebView();
#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif
            builder.Services.AddSingleton<IDbConnectionFactory>(sp => new SqliteConnectionFactory(Path.Combine(Directory.GetCurrentDirectory(), "meineDatenbank.db")));
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
            app.Services.GetRequiredService<DatabaseInitializer>().EnsureCreated();
            return app;
        }
    }
}
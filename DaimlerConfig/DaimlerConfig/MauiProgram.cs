using System;
using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.SignalR.Client;

using DaimlerConfig.Components.Infrastructure;
using DaimlerConfig.Components.Models;
using DaimlerConfig.Components.Repositories;
using DaimlerConfig.Components.Fassade;
using DaimlerConfig.Services;
using Microsoft.Maui.LifecycleEvents;

#if WINDOWS
using Microsoft.UI.Xaml;
#endif

namespace DaimlerConfig
{
    public static class MauiProgram
    {
        // Statischer Zugriff auf die Services
        public static IServiceProvider Services { get; private set; }

        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            // 1. Konfiguration laden
            string benutzerOrdner = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string dateiPfad = Path.Combine(benutzerOrdner, "appsettings.json");
            builder.Configuration.AddJsonFile(dateiPfad, optional: false, reloadOnChange: true);

            // 2. Services registrieren
            builder.Services.AddMauiBlazorWebView();
#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif
            builder.Services.AddSingleton<SignalRService>();
            builder.Services.AddSingleton<DirtyManagerService>();
            builder.Services.AddSingleton<AppLifecycleService>();

            var sqlConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddSingleton<IDbConnectionFactory>(sp =>
                new SqlServerConnectionFactory(sqlConnectionString));

            builder.Services.AddSingleton<IToolRepository, ToolRepository>();
            builder.Services.AddSingleton<IOperationRepository, OperationRepository>();
            builder.Services.AddSingleton<IStationRepository, StationRepository>();
            builder.Services.AddScoped<IRepository<Line>, Repository<Line>>();
            builder.Services.AddScoped<IRepository<StationType>, Repository<StationType>>();
            builder.Services.AddScoped<IRepository<GenerationClass>, Repository<GenerationClass>>();
            builder.Services.AddScoped<IRepository<SavingClass>, Repository<SavingClass>>();
            builder.Services.AddScoped<IRepository<VerificationClass>, Repository<VerificationClass>>();
            builder.Services.AddScoped<IRepository<DecisionClass>, Repository<DecisionClass>>();


            builder.Services.AddScoped<IRepository<ToolClass>, Repository<ToolClass>>();
            builder.Services.AddScoped<IRepository<ToolType>, Repository<ToolType>>();
            builder.Services.AddScoped<IRepository<Template>, Repository<Template>>();


            builder.Services.AddSingleton<Fassade>(sp =>
            {
                var toolRepo = sp.GetRequiredService<IToolRepository>();
                var operationRepo = sp.GetRequiredService<IOperationRepository>();
                var stationRepo = sp.GetRequiredService<IStationRepository>();
                var lineRepo = sp.GetRequiredService<IRepository<Line>>();
                var stationType = sp.GetRequiredService<IRepository<StationType>>();
                var decisionClassRepo = sp.GetRequiredService<IRepository<DecisionClass>>();
                var generationClassRepo = sp.GetRequiredService<IRepository<GenerationClass>>();
                var savingClassRepo = sp.GetRequiredService<IRepository<SavingClass>>();
                var verificationClassRepo = sp.GetRequiredService<IRepository<VerificationClass>>();
                var toolClassRepo = sp.GetRequiredService<IRepository<ToolClass>>();
                var toolTypeRepo = sp.GetRequiredService<IRepository<ToolType>>();
                var templateRepo = sp.GetRequiredService<IRepository<Template>>();
                return new Fassade(toolRepo, operationRepo, stationRepo, lineRepo, stationType, decisionClassRepo, generationClassRepo, savingClassRepo, verificationClassRepo, toolClassRepo, toolTypeRepo, templateRepo);
            });

            // 3. SignalR konfigurieren
            var signalRConfigPfad = Path.Combine(benutzerOrdner, "signalRVPS.json");
            string hubURL = null;
            if (File.Exists(signalRConfigPfad))
            {
                var jsonInhalt = File.ReadAllText(signalRConfigPfad);
                using var jsonDoc = JsonDocument.Parse(jsonInhalt);
                if (jsonDoc.RootElement.TryGetProperty("SignalRHubUrl", out var urlElement))
                {
                    hubURL = urlElement.GetString();
                }
            }

            if (string.IsNullOrWhiteSpace(hubURL))
                throw new Exception("SignalR-URL konnte nicht aus der Konfiguration geladen werden.");

            var connection = new HubConnectionBuilder().WithUrl(hubURL).Build();
            builder.Services.AddSingleton(connection);

            // 4. Lifecycle Event: App schließen abfangen (nur Windows)
            builder.ConfigureLifecycleEvents(events =>
            {
#if WINDOWS
                events.AddWindows(windows =>
                {
                    windows.OnWindowCreated(window =>
                    {
                        // The window parameter is already the native Microsoft.UI.Xaml.Window
                        if (window != null)
                        {
                            window.Closed += async (sender, args) =>
                            {
                                var serviceProvider = Services;
                                var lifecycleService = serviceProvider.GetService<AppLifecycleService>();
                                if (lifecycleService != null)
                                {
                                    await lifecycleService.RaiseAppClosingAsync();
                                }
                            };
                        }
                    });
                });
#endif
            });

            var app = builder.Build();

            // Services-Provider speichern für globalen Zugriff
            Services = app.Services;

            return app;
        }
    }
}
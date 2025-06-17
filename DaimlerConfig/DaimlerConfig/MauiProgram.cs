using System;
using System.IO;
using System.Text.Json;
using DaimlerConfig.Components.Export;
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
using DaimlerConfig.Security;
using MudBlazor.Services;


#if WINDOWS
using Microsoft.UI.Xaml;
#endif

namespace DaimlerConfig
{
    public static class MauiProgram
    {
        // Statischer Zugriff auf die Services
        public static IServiceProvider Services { get; private set; }
        public static string Username { get; private set; } = $"{Environment.UserName};{Guid.NewGuid()}";


        public static string SetzeOderErsetzeGuiId(string identifier)
        {
            if (string.IsNullOrWhiteSpace(identifier))
                throw new ArgumentException("Der Identifier darf nicht leer sein.", nameof(identifier));

            string neueGuiID = Guid.NewGuid().ToString();

            int trennerIndex = identifier.IndexOf(';');

            if (trennerIndex >= 0)
            {
                string nameTeil = identifier.Substring(0, trennerIndex);
                return $"{nameTeil};{neueGuiID}";
            }
            else
            {
                return $"{identifier};{neueGuiID}";
            }
        }

        

        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            

            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            // 1. Konfiguration laden - MIT Verschlüsselung
            string benutzerOrdner = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string dateiPfad = Path.Combine(benutzerOrdner, "appsettings.json");

            // Development Mode Check
            bool developmentMode = false;
#if DEBUG
            developmentMode = false; // In Debug-Builds Verschlüsselung deaktivieren
#endif

            builder.Configuration.AddEncryptedJsonFile(dateiPfad,
                optional: true,
                reloadOnChange: true,
                developmentMode: developmentMode);

            // 2. Services registrieren
            builder.Services.AddMauiBlazorWebView();
#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            // WICHTIG: AppStartupValidationService als Singleton vor anderen Services registrieren
            builder.Services.AddSingleton<AppStartupValidationService>();

            builder.Services.AddSingleton<NavigationStateService>();
            builder.Services.AddSingleton<DirtyManagerService>();
            builder.Services.AddSingleton<AppLifecycleService>();
            builder.Services.AddSingleton<SelectionStateService>();
            builder.Services.AddScoped<SettingsValidationService>();
            builder.Services.AddSingleton<UsernameService>();

            

            builder.Services.AddMudServices();

            // Services immer registrieren, aber mit Fallback-Implementierungen bei Fehlern
            RegisterDataServices(builder);
            RegisterSignalRServices(builder);

            // 4. Lifecycle Event: App schließen abfangen (nur Windows)
            builder.ConfigureLifecycleEvents(events =>
            {
#if WINDOWS
                events.AddWindows(windows =>
                {
                    windows.OnWindowCreated(window =>
                    {
                        if (window != null)
                        {
                            window.Closed += async (sender, args) =>
                            {
                                var serviceProvider = Services;
                                var lifecycleService = serviceProvider.GetService<AppLifecycleService>();
                                if (lifecycleService != null)
                                {
                                    lifecycleService.RaiseAppClosingSync();
                                }
                            };
                        }
                    });
                });
#endif
            });

            // Dispose des temporären ServiceProviders
            // tempServiceProvider.Dispose();

            var app = builder.Build();

            // Services-Provider speichern für globalen Zugriff
            Services = app.Services;
            var usernameService = app.Services.GetService<UsernameService>();

            usernameService.UpdateUsername(SetzeOderErsetzeGuiId(usernameService.Username));

            return app;
        }

        private static void RegisterDataServices(MauiAppBuilder builder)
        {
            try
            {
                var sqlConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");

                if (!string.IsNullOrWhiteSpace(sqlConnectionString))
                {
                    // Prüfung: Validierung durchführen, bevor Services registriert werden
                    var tempConfig = builder.Configuration;
                    var tempValidationService = new AppStartupValidationService(tempConfig);

                    // Nur Services registrieren, wenn keine Validierungsfehler vorliegen
                    if (!tempValidationService.HasErrors)
                    {
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
                        builder.Services.AddScoped<IRepository<ToolVersion>, Repository<ToolVersion>>();
                        builder.Services.AddScoped<IRepository<ToolTypeHasTemplate>, Repository<ToolTypeHasTemplate>>();
                        builder.Services.AddScoped<IRepository<ToolClass>, Repository<ToolClass>>();
                        builder.Services.AddScoped<IRepository<ToolType>, Repository<ToolType>>();
                        builder.Services.AddScoped<IRepository<Template>, Repository<Template>>();
                        builder.Services.AddScoped<IRepository<OperationVersion>, Repository<OperationVersion>>();
                        builder.Services.AddScoped<ExcelExport, ExcelExport>();
                        builder.Services.AddScoped<Language, Language>();

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
                            var toolTypeHasTemplateRepo = sp.GetRequiredService<IRepository<ToolTypeHasTemplate>>();
                            var templateRepo = sp.GetRequiredService<IRepository<Template>>();
                            var export = sp.GetRequiredService<ExcelExport>();
                            var toolversion = sp.GetRequiredService<IRepository<ToolVersion>>();
                            var operationversion = sp.GetRequiredService<IRepository<OperationVersion>>();
                            var language = sp.GetRequiredService<Language>();

                            return new Fassade(toolRepo, operationRepo, stationRepo, lineRepo, stationType, decisionClassRepo,
                                generationClassRepo, savingClassRepo, verificationClassRepo, toolClassRepo, toolTypeRepo,
                                toolTypeHasTemplateRepo, templateRepo, export, toolversion, operationversion, language);
                        });
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"[MauiProgram] Datenbank-Services werden nicht registriert aufgrund von Validierungsfehlern: {tempValidationService.ErrorMessage}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[MauiProgram] Fehler bei DB-Service-Registrierung: {ex.Message}");
                // Services werden nicht registriert - App läuft trotzdem
            }
        }

        private static void RegisterSignalRServices(MauiAppBuilder builder)
        {
            try
            {
                var hubURL = builder.Configuration["SignalR:HubUrl"];

                if (!string.IsNullOrWhiteSpace(hubURL))
                {
                    var connection = new HubConnectionBuilder().WithUrl(hubURL).Build();
                    builder.Services.AddSingleton(connection);
                    builder.Services.AddSingleton<SignalRService>();
                }
                else
                {
                    // Fallback: Dummy-Services
                    builder.Services.AddSingleton<HubConnection>(_ => null!);
                    builder.Services.AddSingleton<SignalRService>(_ => new SignalRService(null!));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[MauiProgram] Fehler bei SignalR-Service-Registrierung: {ex.Message}");
                // Fallback: Dummy-Services
                builder.Services.AddSingleton<HubConnection>(_ => null!);
                builder.Services.AddSingleton<SignalRService>(_ => new SignalRService(null!));
            }
        }
    }
}
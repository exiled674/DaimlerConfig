using Microsoft.Extensions.Logging;
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
               new SqliteConnectionFactory(
                   Path.Combine(FileSystem.Current.AppDataDirectory, "meineDatenbank.db")
               )
            );

            builder.Services.AddSingleton<DatabaseInitializer>();

            builder.Services.AddScoped<IRepository<Station>, StationRepository>();

            var app = builder.Build();

            app.Services
              .GetRequiredService<DatabaseInitializer>()
              .EnsureCreated();

            return app;
        }
    }
}

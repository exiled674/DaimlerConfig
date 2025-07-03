using Microsoft.Extensions.Configuration;
using System;

namespace DConfig.Security
{
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Fügt eine verschlüsselte JSON-Konfigurationsdatei hinzu
        /// </summary>
        public static IConfigurationBuilder AddEncryptedJsonFile(
            this IConfigurationBuilder builder,
            string path,
            bool optional = false,
            bool reloadOnChange = false,
            bool developmentMode = false)
        {
            return builder.Add<EncryptedConfigurationSource>(s =>
            {
                s.FileProvider = null;
                s.Path = path;
                s.Optional = optional;
                s.ReloadOnChange = reloadOnChange;
                s.DevelopmentMode = developmentMode;
                s.ResolveFileProvider();
            });
        }
    }
}
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace ResumeBuilder.Infrastructure.ExternalServices;

/// <summary>
/// Loads configuration from environment variables and environment-specific appsettings files.
/// Supports both local development and cloud deployment.
/// </summary>
public static class ConfigurationExtensions
{
    /// <summary>
    /// Adds configuration from environment variables and appsettings files.
    /// Environment variables take precedence over file-based configuration.
    /// </summary>
    public static IConfigurationBuilder AddEnvironmentConfiguration(
        this IConfigurationBuilder builder,
        IWebHostEnvironment env)
    {
        // Load from appsettings files
        builder
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

        // Load from environment variables (these override appsettings)
        builder.AddEnvironmentVariables();

        return builder;
    }

    /// <summary>
    /// Gets a configuration value, supporting both environment variables and config files.
    /// Useful for lazy evaluation of environment-dependent settings.
    /// </summary>
    public static string? GetValue(this IConfiguration config, string key, string? defaultValue = null)
    {
        // Try environment variable first
        var envValue = Environment.GetEnvironmentVariable(key);
        if (!string.IsNullOrEmpty(envValue))
            return envValue;

        // Fall back to config file
        return config[key] ?? defaultValue;
    }
}

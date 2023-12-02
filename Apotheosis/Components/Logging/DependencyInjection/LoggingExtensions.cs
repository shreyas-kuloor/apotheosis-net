using Apotheosis.Components.Logging.Interfaces;
using Apotheosis.Components.Logging.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Apotheosis.Components.Logging.DependencyInjection;

public static class LoggingExtensions
{
    /// <summary>
    /// Adds logging related services.
    /// </summary>
    /// <param name="services"> The instances of <see cref="IServiceCollection"/>.</param>
    public static void AddLoggingServices(this IServiceCollection services)
    {
        services.AddSingleton(typeof(ILogService<>), typeof(LogService<>));
    }
}
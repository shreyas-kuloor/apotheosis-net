using Apotheosis.Core.Components.Logging.Interfaces;
using Apotheosis.Core.Components.Logging.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Apotheosis.Core.Components.Logging.Extensions;

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
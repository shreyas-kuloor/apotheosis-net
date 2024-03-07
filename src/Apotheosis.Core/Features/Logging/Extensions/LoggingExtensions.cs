using Apotheosis.Core.Features.Logging.Interfaces;
using Apotheosis.Core.Features.Logging.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Apotheosis.Core.Features.Logging.Extensions;

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
using Apotheosis.Core.Features.Converse.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Apotheosis.Core.Features.Converse.Extensions;

public static class ConverseExtensions
{
    /// <summary>
    /// Adds converse command related services.
    /// </summary>
    /// <param name="services"> The instances of <see cref="IServiceCollection"/>.</param>
    /// <param name="configuration">An instance of <see cref="IConfiguration"/>.</param>
    public static void AddConverseServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<ConverseSettings>(configuration.GetSection(ConverseSettings.Name));
    }
}
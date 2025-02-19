using Apotheosis.Core.Features.FeatureFlags.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Apotheosis.Core.Features.FeatureFlags.Extensions;

public static class FeatureFlagExtensions
{
    /// <summary>
    /// Adds feature flag related services.
    /// </summary>
    /// <param name="services"> The instances of <see cref="IServiceCollection"/>.</param>
    /// <param name="configuration">An instance of <see cref="IConfiguration"/>.</param>
    public static void AddFeatureFlagServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<FeatureFlagSettings>(configuration.GetSection(FeatureFlagSettings.Name));
    }
}
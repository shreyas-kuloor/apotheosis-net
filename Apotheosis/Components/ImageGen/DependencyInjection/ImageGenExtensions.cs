using Apotheosis.Components.ImageGen.Configuration;
using Apotheosis.Components.ImageGen.Interfaces;
using Apotheosis.Components.ImageGen.Network;
using Apotheosis.Components.ImageGen.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;

namespace Apotheosis.Components.ImageGen.DependencyInjection;

public static class ImageGenExtensions
{
    /// <summary>
    /// Adds client related services.
    /// </summary>
    /// <param name="services"> The instances of <see cref="IServiceCollection"/>.</param>
    /// <param name="imageGenSection">An instance of <see cref="IConfigurationSection"/>.</param>
    public static void AddImageGenServices(
        this IServiceCollection services,
        IConfigurationSection imageGenSection)
    {
        services.Configure<ImageGenSettings>(imageGenSection);
        AddHttpClient<IImageGenNetworkDriver, StableDiffusionNetworkDriver>(services);
        services.AddScoped<IImageGenService, ImageGenService>();
    }

    private static void AddHttpClient<TClient, TImplementation>(
        IServiceCollection services)
        where TClient : class
        where TImplementation : class, TClient
    {
        services.AddHttpClient<TClient, TImplementation>()
            .AddPolicyHandler((handlerServices, _) => HttpPolicyExtensions
                .HandleTransientHttpError()
                .RetryAsync(
                    StableDiffusionNetworkDriver.Retries,
                    (result, _) =>
                    {
                        var logger = handlerServices.GetService<ILogger<StableDiffusionNetworkDriver>>();
                        logger?.LogError(result.Exception, "[{Source}] {Message}", nameof(StableDiffusionNetworkDriver), result.Result?.ToString());
                    }))
            .AddPolicyHandler(_ => Policy.TimeoutAsync<HttpResponseMessage>(StableDiffusionNetworkDriver.Timeout));
    }
}
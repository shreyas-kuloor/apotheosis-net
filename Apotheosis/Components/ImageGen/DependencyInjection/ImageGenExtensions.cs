using Apotheosis.Components.ImageGen.Configuration;
using Apotheosis.Components.ImageGen.Interfaces;
using Apotheosis.Components.ImageGen.Network;
using Apotheosis.Components.ImageGen.Services;
using Apotheosis.Components.Logging.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;

namespace Apotheosis.Components.ImageGen.DependencyInjection;

public static class ImageGenExtensions
{
    /// <summary>
    /// Adds image generation related services.
    /// </summary>
    /// <param name="services"> The instances of <see cref="IServiceCollection"/>.</param>
    /// <param name="imageGenSection">An instance of <see cref="IConfigurationSection"/>.</param>
    public static void AddImageGenServices(
        this IServiceCollection services,
        IConfigurationSection imageGenSection)
    {
        var imageGenSettings = imageGenSection.Get<ImageGenSettings>()!;
        services.AddSingleton(imageGenSettings);
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
                        var logger = handlerServices.GetService<ILogService<StableDiffusionNetworkDriver>>();
                        logger?.LogError(result.Exception, result.Result?.ToString());
                    }))
            .AddPolicyHandler(_ => Policy.TimeoutAsync<HttpResponseMessage>(StableDiffusionNetworkDriver.Timeout));
    }
}
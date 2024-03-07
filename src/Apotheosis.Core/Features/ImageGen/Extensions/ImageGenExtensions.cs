using Apotheosis.Core.Features.ImageGen.Configuration;
using Apotheosis.Core.Features.ImageGen.Interfaces;
using Apotheosis.Core.Features.ImageGen.Network;
using Apotheosis.Core.Features.Logging.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;

namespace Apotheosis.Core.Features.ImageGen.Extensions;

public static class ImageGenExtensions
{
    /// <summary>
    /// Adds image generation related services.
    /// </summary>
    /// <param name="services"> The instances of <see cref="IServiceCollection"/>.</param>
    /// <param name="configuration">An instance of <see cref="IConfiguration"/>.</param>
    public static void AddImageGenServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<ImageGenSettings>(configuration.GetSection(ImageGenSettings.Name));
        AddHttpClient<IImageGenNetworkDriver, StableDiffusionNetworkDriver>(services);
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
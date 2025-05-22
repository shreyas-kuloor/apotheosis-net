using Apotheosis.Core.Features.MediaRequest.Interfaces;
using Apotheosis.Server.Features.MediaRequest.Configuration;
using Apotheosis.Server.Features.MediaRequest.Network;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;

namespace Apotheosis.Server.Features.MediaRequest.Extensions;

public static class MediaRequestExtensions
{
    /// <summary>
    /// Adds media request related services.
    /// </summary>
    /// <param name="services"> The instances of <see cref="IServiceCollection"/>.</param>
    /// <param name="configuration">An instance of <see cref="IConfiguration"/>.</param>
    public static void AddMediaRequestServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<MediaRequestSettings>(configuration.GetSection(MediaRequestSettings.Name));
        services.Configure<RadarrSettings>(configuration.GetSection(RadarrSettings.Name));
        services.Configure<SonarrSettings>(configuration.GetSection(SonarrSettings.Name));

        services.AddHttpClient<IRadarrClient, RadarrClient>()
            .AddPolicyHandler((handlerServices, _) => HttpPolicyExtensions
                .HandleTransientHttpError()
                .RetryAsync(
                    RadarrClient.Retries,
                    (result, _) =>
                    {
                        var logger = handlerServices.GetService<ILogger<RadarrClient>>();
                        logger?.LogError(result.Exception, "Error: {Error}", result.Result?.ToString());
                    }))
            .AddPolicyHandler(_ => Policy.TimeoutAsync<HttpResponseMessage>(RadarrClient.Timeout));

        services.AddHttpClient<ISonarrClient, SonarrClient>()
            .AddPolicyHandler((handlerServices, _) => HttpPolicyExtensions
                .HandleTransientHttpError()
                .RetryAsync(
                    SonarrClient.Retries,
                    (result, _) =>
                    {
                        var logger = handlerServices.GetService<ILogger<SonarrClient>>();
                        logger?.LogError(result.Exception, "Error: {Error}", result.Result?.ToString());
                    }))
            .AddPolicyHandler(_ => Policy.TimeoutAsync<HttpResponseMessage>(SonarrClient.Timeout));
    }
}
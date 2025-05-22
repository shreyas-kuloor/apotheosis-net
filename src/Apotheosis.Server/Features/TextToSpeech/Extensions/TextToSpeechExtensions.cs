using Apotheosis.Core.Features.TextToSpeech.Interfaces;
using Apotheosis.Server.Features.TextToSpeech.Configuration;
using Apotheosis.Server.Features.TextToSpeech.Network;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;

namespace Apotheosis.Server.Features.TextToSpeech.Extensions;

public static class TextToSpeechExtensions
{
    /// <summary>
    /// Adds client related services.
    /// </summary>
    /// <param name="services"> The instances of <see cref="IServiceCollection"/>.</param>
    /// <param name="configuration">An instance of <see cref="IConfiguration"/>.</param>
    public static void AddTextToSpeechServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<TextToSpeechSettings>(configuration.GetSection(TextToSpeechSettings.Name));
        AddHttpClient<ITextToSpeechNetworkDriver, ElevenLabsNetworkDriver>(services);
    }

    static void AddHttpClient<TClient, TImplementation>(
        IServiceCollection services)
        where TClient : class
        where TImplementation : class, TClient
    {
        services.AddHttpClient<TClient, TImplementation>()
            .AddPolicyHandler((handlerServices, _) => HttpPolicyExtensions
                .HandleTransientHttpError()
                .RetryAsync(
                    ElevenLabsNetworkDriver.Retries,
                    (result, _) =>
                    {
                        var logger = handlerServices.GetService<ILogger<ElevenLabsNetworkDriver>>();
                        logger?.LogError(result.Exception, "Error: {Error}", result.Result?.ToString());
                    }))
            .AddPolicyHandler(_ => Policy.TimeoutAsync<HttpResponseMessage>(ElevenLabsNetworkDriver.Timeout));
    }
}
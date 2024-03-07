using Apotheosis.Core.Features.Logging.Interfaces;
using Apotheosis.Core.Features.TextToSpeech.Configuration;
using Apotheosis.Core.Features.TextToSpeech.Interfaces;
using Apotheosis.Core.Features.TextToSpeech.Network;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;

namespace Apotheosis.Core.Features.TextToSpeech.Extensions;

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

    private static void AddHttpClient<TClient, TImplementation>(
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
                        var logger = handlerServices.GetService<ILogService<ElevenLabsNetworkDriver>>();
                        logger?.LogError(result.Exception, result.Result?.ToString());
                    }))
            .AddPolicyHandler(_ => Policy.TimeoutAsync<HttpResponseMessage>(ElevenLabsNetworkDriver.Timeout));
    }
}
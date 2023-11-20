using Apotheosis.Components.TextToSpeech.Configuration;
using Apotheosis.Components.TextToSpeech.Interfaces;
using Apotheosis.Components.TextToSpeech.Network;
using Apotheosis.Components.TextToSpeech.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;

namespace Apotheosis.Components.TextToSpeech.DependencyInjection;

public static class TextToSpeechExtensions
{
    /// <summary>
    /// Adds client related services.
    /// </summary>
    /// <param name="services"> The instances of <see cref="IServiceCollection"/>.</param>
    /// <param name="textToSpeechSection">An instance of <see cref="IConfigurationSection"/>.</param>
    public static void AddTextToSpeechServices(
        this IServiceCollection services,
        IConfigurationSection textToSpeechSection)
    {
        services.Configure<TextToSpeechSettings>(textToSpeechSection);
        AddHttpClient<ITextToSpeechNetworkDriver, ElevenLabsNetworkDriver>(services);
        services.AddScoped<ITextToSpeechService, TextToSpeechService>();
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
                        var logger = handlerServices.GetService<ILogger<ElevenLabsNetworkDriver>>();
                        logger?.LogError(result.Exception, "[{Source}] {Message}", nameof(ElevenLabsNetworkDriver), result.Result?.ToString());
                    }))
            .AddPolicyHandler(_ => Policy.TimeoutAsync<HttpResponseMessage>(ElevenLabsNetworkDriver.Timeout));
    }
}
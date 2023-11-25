using Apotheosis.Components.AiChat.Configuration;
using Apotheosis.Components.AiChat.Interfaces;
using Apotheosis.Components.AiChat.Network;
using Apotheosis.Components.AiChat.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;

namespace Apotheosis.Components.AiChat.DependencyInjection;

public static class AiChatExtensions
{
    /// <summary>
    /// Adds client related services.
    /// </summary>
    /// <param name="services"> The instances of <see cref="IServiceCollection"/>.</param>
    /// <param name="aiChatSection">An instance of <see cref="IConfigurationSection"/>.</param>
    public static void AddAiChatServices(
        this IServiceCollection services,
        IConfigurationSection aiChatSection)
    {
        services.Configure<AiChatSettings>(aiChatSection);
        AddHttpClient<IAiChatNetworkDriver, OpenAiNetworkDriver>(services);
        services.AddSingleton<IAiThreadChannelRepository, AiThreadChannelRepository>();
        services.AddScoped<IAiChatService, AiChatService>();
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
                    OpenAiNetworkDriver.Retries,
                    (result, _) =>
                    {
                        var logger = handlerServices.GetService<ILogger<OpenAiNetworkDriver>>();
                        logger?.LogError(result.Exception, "[{Source}] {Message}", nameof(OpenAiNetworkDriver), result.Result?.ToString());
                    }))
            .AddPolicyHandler(_ => Policy.TimeoutAsync<HttpResponseMessage>(OpenAiNetworkDriver.Timeout));
    }
}
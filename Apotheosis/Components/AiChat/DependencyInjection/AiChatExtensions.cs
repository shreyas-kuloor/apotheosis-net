using Apotheosis.Components.AiChat.Configuration;
using Apotheosis.Components.AiChat.Interfaces;
using Apotheosis.Components.AiChat.Network;
using Apotheosis.Components.AiChat.Services;
using Apotheosis.Components.Logging.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;

namespace Apotheosis.Components.AiChat.DependencyInjection;

public static class AiChatExtensions
{
    /// <summary>
    /// Adds AI chat related services.
    /// </summary>
    /// <param name="services"> The instances of <see cref="IServiceCollection"/>.</param>
    /// <param name="aiChatSection">An instance of <see cref="IConfigurationSection"/>.</param>
    public static void AddAiChatServices(
        this IServiceCollection services,
        IConfigurationSection aiChatSection)
    {
        var aiChatSettings = aiChatSection.Get<AiChatSettings>()!;
        services.AddSingleton(aiChatSettings);
        AddHttpClient<IAiChatNetworkDriver, OpenAiNetworkDriver>(services);
        services.AddSingleton<IAiThreadChannelRepository, AiThreadChannelRepository>();
        services.AddScoped<IAiChatService, AiChatService>();
        services.AddSingleton<IAiChatThreadMessageHandler, AiChatThreadMessageHandler>();
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
                        var logger = handlerServices.GetService<ILogService<OpenAiNetworkDriver>>();
                        logger?.LogError(result.Exception, result.Result?.ToString());
                    }))
            .AddPolicyHandler(_ => Policy.TimeoutAsync<HttpResponseMessage>(OpenAiNetworkDriver.Timeout));
    }
}
using Apotheosis.Core.Features.AiChat.Interfaces;
using Apotheosis.Server.Features.AiChat.Configuration;
using Apotheosis.Server.Features.AiChat.Network;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;

namespace Apotheosis.Server.Features.AiChat.Extensions;

public static class AiChatExtensions
{
    /// <summary>
    /// Adds AI chat related services.
    /// </summary>
    /// <param name="services"> The instances of <see cref="IServiceCollection"/>.</param>
    /// <param name="configuration">An instance of <see cref="IConfiguration"/>.</param>
    public static void AddAiChatServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<AiChatSettings>(configuration.GetSection(AiChatSettings.Name));
        AddHttpClient<IAiChatClient, AiChatClient>(services);
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
                    AiChatClient.Retries,
                    (result, _) =>
                    {
                        var logger = handlerServices.GetService<ILogger<AiChatClient>>();
                        logger?.LogError(result.Exception, "Error: {Error}", result.Result?.ToString());
                    }))
            .AddPolicyHandler(_ => Policy.TimeoutAsync<HttpResponseMessage>(AiChatClient.Timeout));
    }
}
using Apotheosis.Core.Features.AiChat.Configuration;
using Apotheosis.Core.Features.AiChat.Interfaces;
using Apotheosis.Core.Features.AiChat.Network;
using Apotheosis.Core.Features.Logging.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;

namespace Apotheosis.Core.Features.AiChat.Extensions;

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

    private static void AddHttpClient<TClient, TImplementation>(
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
                        var logger = handlerServices.GetService<ILogService<AiChatClient>>();
                        logger?.LogError(result.Exception, result.Result?.ToString());
                    }))
            .AddPolicyHandler(_ => Policy.TimeoutAsync<HttpResponseMessage>(AiChatClient.Timeout));
    }
}
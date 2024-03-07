using Apotheosis.Core.Features.GcpDot.Configuration;
using Apotheosis.Core.Features.GcpDot.Interfaces;
using Apotheosis.Core.Features.GcpDot.Network;
using Apotheosis.Core.Features.Logging.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;

namespace Apotheosis.Core.Features.GcpDot.Extensions;

public static class GcpDotExtensions
{
    /// <summary>
    /// Adds client related services.
    /// </summary>
    /// <param name="services"> The instances of <see cref="IServiceCollection"/>.</param>
    /// <param name="configuration">An instance of <see cref="IConfiguration"/>.</param>
    public static void AddGcpDotServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<GcpDotSettings>(configuration.GetSection(GcpDotSettings.Name));
        AddHttpClient<IGcpDotNetworkDriver, GcpDotNetworkDriver>(services);
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
                    GcpDotNetworkDriver.Retries,
                    (result, _) =>
                    {
                        var logger = handlerServices.GetService<ILogService<GcpDotNetworkDriver>>();
                        logger?.LogError(result.Exception, result.Result?.ToString());
                    }))
            .AddPolicyHandler(_ => Policy.TimeoutAsync<HttpResponseMessage>(GcpDotNetworkDriver.Timeout));
    }
}
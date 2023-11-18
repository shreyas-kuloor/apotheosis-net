using Apotheosis.Components.GCPDot.Configuration;
using Apotheosis.Components.GCPDot.Interfaces;
using Apotheosis.Components.GCPDot.Network;
using Apotheosis.Components.GCPDot.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;

namespace Apotheosis.Components.GCPDot.DependencyInjection;

public static class GcpDotExtensions
{
    /// <summary>
    /// Adds client related services.
    /// </summary>
    /// <param name="services"> The instances of <see cref="IServiceCollection"/>.</param>
    /// <param name="gcpDotSection">An instance of <see cref="IConfigurationSection"/>.</param>
    public static void AddGcpDotServices(
        this IServiceCollection services,
        IConfigurationSection gcpDotSection)
    {
        services.Configure<GcpDotSettings>(gcpDotSection);
        AddHttpClient<IGcpDotNetworkDriver, GcpDotNetworkDriver>(services);
        services.AddScoped<IGcpDotService, GcpDotService>();
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
                        var logger = handlerServices.GetService<ILogger<GcpDotNetworkDriver>>();
                        logger?.LogError(result.Exception, "[{Source}] {Message}", nameof(GcpDotNetworkDriver), result.Result?.ToString());
                    }))
            .AddPolicyHandler(_ => Policy.TimeoutAsync<HttpResponseMessage>(GcpDotNetworkDriver.Timeout));
    }
}
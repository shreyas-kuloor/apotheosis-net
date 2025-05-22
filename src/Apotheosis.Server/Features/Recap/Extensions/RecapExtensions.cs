using Apotheosis.Core.Features.Recap.Interfaces;
using Apotheosis.Server.Features.Recap.Configuration;
using Apotheosis.Server.Features.Recap.Network;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;

namespace Apotheosis.Server.Features.Recap.Extensions;

public static class RecapExtensions
{
    /// <summary>
    /// Adds recap related services.
    /// </summary>
    /// <param name="services"> The instances of <see cref="IServiceCollection"/>.</param>
    /// <param name="configuration">An instance of <see cref="IConfiguration"/>.</param>
    public static void AddRecapServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<LeagueRecapSettings>(configuration.GetSection(LeagueRecapSettings.Name));

        services.AddHttpClient<ILeagueRecapClient, LeagueRecapClient>()
            .AddPolicyHandler((handlerServices, _) => HttpPolicyExtensions
                .HandleTransientHttpError()
                .RetryAsync(
                    LeagueRecapClient.Retries,
                    (result, _) =>
                    {
                        var logger = handlerServices.GetService<ILogger<LeagueRecapClient>>();
                        logger?.LogError(result.Exception, "Error: {Error}", result.Result?.ToString());
                    }))
            .AddPolicyHandler(_ => Policy.TimeoutAsync<HttpResponseMessage>(LeagueRecapClient.Timeout));
    }
}
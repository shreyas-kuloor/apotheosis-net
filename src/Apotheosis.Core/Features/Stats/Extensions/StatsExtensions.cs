using Apotheosis.Core.Features.Logging.Interfaces;
using Apotheosis.Core.Features.Stats.Configuration;
using Apotheosis.Core.Features.Stats.Interfaces;
using Apotheosis.Core.Features.Stats.Network;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;

namespace Apotheosis.Core.Features.Stats.Extensions;

public static class StatsExtensions
{
    /// <summary>
    /// Adds stats related services.
    /// </summary>
    /// <param name="services"> The instances of <see cref="IServiceCollection"/>.</param>
    /// <param name="configuration">An instance of <see cref="IConfiguration"/>.</param>
    public static void AddStatsServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<LeagueStatsSettings>(configuration.GetSection(LeagueStatsSettings.Name));

        services.AddHttpClient<ILeagueStatsClient, LeagueStatsClient>()
            .AddPolicyHandler((handlerServices, _) => HttpPolicyExtensions
                .HandleTransientHttpError()
                .RetryAsync(
                    LeagueStatsClient.Retries,
                    (result, _) =>
                    {
                        var logger = handlerServices.GetService<ILogService<LeagueStatsClient>>();
                        logger?.LogError(result.Exception, result.Result?.ToString());
                    }))
            .AddPolicyHandler(_ => Policy.TimeoutAsync<HttpResponseMessage>(LeagueStatsClient.Timeout));
    }
}
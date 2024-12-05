using Apotheosis.Core.Features.Logging.Interfaces;
using Apotheosis.Core.Features.Rank.Configuration;
using Apotheosis.Core.Features.Rank.Interfaces;
using Apotheosis.Core.Features.Rank.Network;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;

namespace Apotheosis.Core.Features.Rank.Extensions;

public static class RankExtensions
{
    /// <summary>
    /// Adds media request related services.
    /// </summary>
    /// <param name="services"> The instances of <see cref="IServiceCollection"/>.</param>
    /// <param name="configuration">An instance of <see cref="IConfiguration"/>.</param>
    public static void AddRankServices(
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
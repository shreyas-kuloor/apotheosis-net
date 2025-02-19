using Apotheosis.Core.Features.FeatureFlags.Configuration;
using Apotheosis.Core.Features.Logging.Interfaces;
using Apotheosis.Core.Features.Stats.Exceptions;
using Apotheosis.Core.Features.Stats.Interfaces;
using Microsoft.Extensions.Logging;
using NetCord.Rest;

namespace Apotheosis.Core.Features.Stats.Modules;
public sealed class StatsModule(
    ILeagueStatsService leagueStatsService,
    ILogService<StatsModule> logger,
    IOptions<FeatureFlagSettings> featureFlagOptions) : ApplicationCommandModule<SlashCommandContext>
{
    readonly FeatureFlagSettings featureFlagSettings = featureFlagOptions.Value;

    [SlashCommand("stats", "Get the League of Legends stats of the summoner with the specified name and tag")]
    public async Task GetStatsAsync(string name, string tag)
    {
        if (!featureFlagSettings.StatsEnabled)
        {
            await RespondAsync(InteractionCallback.Message(
                new InteractionMessageProperties()
                .WithContent("This command is currently disabled. Please try again later!")
                .WithFlags(MessageFlags.Ephemeral)));
            return;
        }

        await RespondAsync(InteractionCallback.DeferredMessage());
        logger.Log(LogLevel.Information, null, $"{Context.User.GlobalName} used /stats {name} {tag}");

        string stats;
        try
        {
            stats = await leagueStatsService.GetSummonerStatsAsync(name, tag);
        }
        catch (LeagueStatsNetworkException)
        {
            await FollowupAsync(
                new InteractionMessageProperties()
                .WithContent("The provided summoner does not exist or there was a problem retrieving their details."));

            return;
        }

        if (string.IsNullOrEmpty(stats))
        {
            await FollowupAsync(
                new InteractionMessageProperties()
                .WithContent("The provided summoner does not exist or has not played a game recently."));

            return;
        }

        await FollowupAsync(
            new InteractionMessageProperties()
            .WithContent(stats));
    }
}

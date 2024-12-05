using Apotheosis.Core.Features.FeatureFlags.Configuration;
using Apotheosis.Core.Features.Logging.Interfaces;
using Apotheosis.Core.Features.Rank.Exceptions;
using Apotheosis.Core.Features.Rank.Interfaces;
using Apotheosis.Core.Features.Rank.Models;
using Microsoft.Extensions.Logging;
using NetCord.Rest;

namespace Apotheosis.Core.Features.Rank.Modules;
public sealed class RankModule(
    ILeagueStatsService leagueStatsService,
    ILogService<RankModule> logger,
    IOptions<FeatureFlagSettings> featureFlagOptions) : ApplicationCommandModule<SlashCommandContext>
{
    readonly FeatureFlagSettings featureFlagSettings = featureFlagOptions.Value;

    [SlashCommand("rank", "Get the League of Legends rank of the summoner with the specified name and tag")]
    public async Task GetRankAsync(string name, string tag)
    {
        if (!featureFlagSettings.RankEnabled)
        {
            await RespondAsync(InteractionCallback.Message(
                new InteractionMessageProperties()
                .WithContent("This command is currently disabled. Please try again later!")
                .WithFlags(MessageFlags.Ephemeral)));
            return;
        }

        await RespondAsync(InteractionCallback.DeferredMessage());
        logger.Log(LogLevel.Information, null, $"{Context.User.GlobalName} used /rank {name} {tag}");

        RankDetailsDto? rankSoloDuoDetails;
        try
        {
            rankSoloDuoDetails = await leagueStatsService.GetRankedSoloStatsAsync(name, tag);
        }
        catch (LeagueStatsNetworkException)
        {
            await FollowupAsync(
                new InteractionMessageProperties()
                .WithContent("The provided summoner does not exist or there was a problem retrieving their details."));

            return;
        }

        if (rankSoloDuoDetails == null)
        {
            await FollowupAsync(
                new InteractionMessageProperties()
                .WithContent("The provided summoner is unranked in Ranked Solo/Duo."));

            return;
        }

        await FollowupAsync(
            new InteractionMessageProperties()
            .WithEmbeds([BuildRankEmbed(name, tag, rankSoloDuoDetails)]));
    }

    static EmbedProperties BuildRankEmbed(string name, string tag, RankDetailsDto rankDetails)
    {
        var embed = new EmbedProperties()
            .WithTitle($"{name} #{tag}")
            .WithColor(new Color(10, 50, 60));

        var fields = new List<EmbedFieldProperties>
        {
            new() { Name = "Queue", Value = "Ranked Solo/Duo" },
            new() { Name = "Rank", Value = $"{rankDetails.Tier} {rankDetails.Rank} - {rankDetails.LeaguePoints} LP" },
            new() { Name = "Wins", Value = $"{rankDetails.Wins}" },
            new() { Name = "Losses", Value = $"{rankDetails.Losses}" }
        };

        if (fields.Count > 0)
        {
            embed.AddFields(fields);
        }

        return embed;
    }
}

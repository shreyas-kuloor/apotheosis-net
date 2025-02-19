using Apotheosis.Core.Features.FeatureFlags.Configuration;
using Apotheosis.Core.Features.Logging.Interfaces;
using Apotheosis.Core.Features.Recap.Exceptions;
using Apotheosis.Core.Features.Recap.Interfaces;
using Microsoft.Extensions.Logging;
using NetCord.Rest;

namespace Apotheosis.Core.Features.Recap.Modules;
public sealed class RecapModule(
    ILeagueRecapService leagueRecapService,
    ILogService<RecapModule> logger,
    IOptions<FeatureFlagSettings> featureFlagOptions) : ApplicationCommandModule<SlashCommandContext>
{
    readonly FeatureFlagSettings featureFlagSettings = featureFlagOptions.Value;

    [SlashCommand("recap", "Get the League of Legends recap of the summoner with the specified name and tag")]
    public async Task GetRecapAsync(string name, string tag)
    {
        if (!featureFlagSettings.RecapEnabled)
        {
            await RespondAsync(InteractionCallback.Message(
                new InteractionMessageProperties()
                .WithContent("This command is currently disabled. Please try again later!")
                .WithFlags(MessageFlags.Ephemeral)));
            return;
        }

        await RespondAsync(InteractionCallback.DeferredMessage());
        logger.Log(LogLevel.Information, null, $"{Context.User.GlobalName} used /recap {name} {tag}");

        string recap;
        try
        {
            recap = await leagueRecapService.GetSummonerRecapAsync(name, tag);
        }
        catch (LeagueRecapNetworkException)
        {
            await FollowupAsync(
                new InteractionMessageProperties()
                .WithContent("The provided summoner does not exist or there was a problem retrieving their details."));

            return;
        }

        if (string.IsNullOrEmpty(recap))
        {
            await FollowupAsync(
                new InteractionMessageProperties()
                .WithContent("The provided summoner does not exist or has not played a game recently."));

            return;
        }

        await FollowupAsync(
            new InteractionMessageProperties()
            .WithContent(recap));
    }
}

using Apotheosis.Core.Features.Recap.Interfaces;
using Apotheosis.Server.Features.Recap.Exceptions;
using Microsoft.Extensions.Logging;

namespace Apotheosis.Server.Features.Recap.Modules;
public sealed class RecapModule(
    ILeagueRecapService leagueRecapService,
    ILogger<RecapModule> logger,
    IOptions<FeatureFlagSettings> featureFlagOptions) : ApplicationCommandModule<SlashCommandContext>
{
    readonly FeatureFlagSettings _featureFlagSettings = featureFlagOptions.Value;

    [SlashCommand("recap", "Get the League of Legends recap of the summoner with the specified name and tag")]
    public async Task GetRecapAsync(string name, string tag)
    {
        if (!_featureFlagSettings.RecapEnabled)
        {
            await RespondAsync(InteractionCallback.Message(
                new InteractionMessageProperties()
                .WithContent("This command is currently disabled. Please try again later!")
                .WithFlags(MessageFlags.Ephemeral)));
            return;
        }

        await RespondAsync(InteractionCallback.DeferredMessage());
        logger.LogInformation("{User} used /recap {Name} {Tag}", Context.User.GlobalName, name, tag);

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

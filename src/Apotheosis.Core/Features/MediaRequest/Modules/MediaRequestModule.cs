using Apotheosis.Core.Features.FeatureFlags.Configuration;
using Apotheosis.Core.Features.Logging.Interfaces;
using Apotheosis.Core.Features.MediaRequest.Interfaces;
using Microsoft.Extensions.Logging;
using NetCord.Rest;

namespace Apotheosis.Core.Features.MediaRequest.Modules;

[SlashCommand("request", "Request movies and shows to be added to Plex.")]
public sealed class MediaRequestModule(
    IMediaRequestService mediaRequestService,
    ILogService<MediaRequestModule> logger,
    IOptions<FeatureFlagSettings> featureFlagOptions) : ApplicationCommandModule<SlashCommandContext>
{
    readonly FeatureFlagSettings featureFlagSettings = featureFlagOptions.Value;

    [SubSlashCommand("movie", "Request a movie to be added to Plex.")]
    public async Task RequestMovieAsync(string term)
    {
        if (!featureFlagSettings.RequestMovieEnabled)
        {
            await RespondAsync(InteractionCallback.Message(
                new InteractionMessageProperties()
                .WithContent("This command is currently disabled. Please try again later!")
                .WithFlags(MessageFlags.Ephemeral)));
            return;
        }

        logger.Log(LogLevel.Information, null, $"{Context.User.GlobalName} used /request movie {term}");

        await RespondAsync(InteractionCallback.DeferredMessage(MessageFlags.Ephemeral));
        var movies = await mediaRequestService.SearchMovies(term);

        var options = movies.Select(movie => 
            new StringMenuSelectOptionProperties(movie.Title, $"{movie.TmdbId}").WithDescription($"{movie.Year}"));

        await FollowupAsync(new InteractionMessageProperties()
            .WithContent("Choose one of the following movies:")
            .WithFlags(MessageFlags.Ephemeral)
            .AddComponents(
                new StringMenuProperties(
                    "movie-menu",
                    options)
                .WithPlaceholder("Select a movie")));
    }

    [SubSlashCommand("series", "Request a series to be added to Plex.")]
    public async Task RequestSeriesAsync(string term)
    {
        if (!featureFlagSettings.RequestSeriesEnabled)
        {
            await RespondAsync(InteractionCallback.Message(
                new InteractionMessageProperties()
                .WithContent("This command is currently disabled. Please try again later!")
                .WithFlags(MessageFlags.Ephemeral)));
            return;
        }

        logger.Log(LogLevel.Information, null, $"{Context.User.GlobalName} used /request series {term}");

        await RespondAsync(InteractionCallback.DeferredMessage(MessageFlags.Ephemeral));
        var series = await mediaRequestService.SearchSeries(term);

        var options = series.Select(s =>
            new StringMenuSelectOptionProperties(s.Title, $"{s.TvdbId}").WithDescription($"{s.Year}"));

        await FollowupAsync(new InteractionMessageProperties()
            .WithContent("Choose one of the following series:")
            .WithFlags(MessageFlags.Ephemeral)
            .AddComponents(
                new StringMenuProperties(
                    "series-menu",
                    options)
                .WithPlaceholder("Select a series")));
    }
}

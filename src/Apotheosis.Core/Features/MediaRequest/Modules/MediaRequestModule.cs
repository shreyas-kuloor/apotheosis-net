using Apotheosis.Core.Features.MediaRequest.Interfaces;
using NetCord.Rest;

namespace Apotheosis.Core.Features.MediaRequest.Modules;

[SlashCommand("request", "Request movies and shows to be added to Plex.")]
public sealed class MediaRequestModule(IMediaRequestService mediaRequestService) : ApplicationCommandModule<SlashCommandContext>
{
    [SubSlashCommand("movie", "Request a movie to be added to Plex.")]
    public async Task RequestMovieAsync(string term)
    {
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

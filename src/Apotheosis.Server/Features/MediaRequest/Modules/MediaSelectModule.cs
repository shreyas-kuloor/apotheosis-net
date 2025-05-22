using Apotheosis.Core.Features.MediaRequest.Interfaces;
using Apotheosis.Core.Features.MediaRequest.Models;

namespace Apotheosis.Server.Features.MediaRequest.Modules;
public sealed class MediaSelectModule(
    IActiveMediaSelectionCache activeMediaSelectionCache, 
    IMediaRequestCache mediaRequestCache,
    IMediaRequestMessageCache mediaRequestMessageCache) : ComponentInteractionModule<StringMenuInteractionContext>
{
    [ComponentInteraction("movie-menu")]
    public async Task MovieMenuAsync()
    {
        await RespondAsync(InteractionCallback.DeferredMessage(MessageFlags.Ephemeral));
        var movieChoiceId = Context.SelectedValues.AsEnumerable().FirstOrDefault()!;
        var choice = mediaRequestCache.GetCachedMovie(int.Parse(movieChoiceId));

        if (choice == null)
        {
            await FollowupAsync(
                new InteractionMessageProperties()
                .WithContent("Something went wrong when fetching movie details.")
                .WithFlags(MessageFlags.Ephemeral));
            return;
        }

        var message = await FollowupAsync(
            new InteractionMessageProperties()
            .WithContent($"Request this movie?")
            .WithFlags(MessageFlags.Ephemeral)
            .WithEmbeds([BuildMovieEmbed(choice)])
            .WithComponents([CreateButtonFromStatus("req-movie-button", choice.Status)]));

        activeMediaSelectionCache.StoreActiveMovieSelection(message.Id, choice);
        mediaRequestMessageCache.StoreAssociatedMediaRequestMessages(Context.Message.Id, message.Id);
    }

    [ComponentInteraction("series-menu")]
    public async Task SeriesMenuAsync()
    {
        await RespondAsync(InteractionCallback.DeferredMessage(MessageFlags.Ephemeral));
        var seriesChoiceId = Context.SelectedValues.AsEnumerable().FirstOrDefault()!;
        var choice = mediaRequestCache.GetCachedSeries(int.Parse(seriesChoiceId));

        if (choice == null)
        {
            await FollowupAsync(
                new InteractionMessageProperties()
                .WithContent("Something went wrong when fetching series details.")
                .WithFlags(MessageFlags.Ephemeral));
            return;
        }

        var message = await FollowupAsync(
            new InteractionMessageProperties()
            .WithContent("Request this series?")
            .WithFlags(MessageFlags.Ephemeral)
            .WithEmbeds([BuildSeriesEmbed(choice)])
            .WithComponents([CreateButtonFromStatus("req-series-button", choice.Status)]));

        activeMediaSelectionCache.StoreActiveSeriesSelection(message.Id, choice);
        mediaRequestMessageCache.StoreAssociatedMediaRequestMessages(Context.Message.Id, message.Id);
    }

    static ActionRowProperties CreateButtonFromStatus(string buttonId, MediaStatus status)
    {
        var buttonText = status switch
        {
            MediaStatus.Available => "Already Added",
            MediaStatus.Processing => "Already Processing",
            MediaStatus.NotAdded => "Request",
            _ => "Request"
        };

        var buttonStyle = (status == MediaStatus.Processing || status == MediaStatus.NotAdded) ? ButtonStyle.Primary : ButtonStyle.Success;

        return new ActionRowProperties([new ButtonProperties(buttonId, buttonText, buttonStyle) { Disabled = !(status == MediaStatus.NotAdded) }]);
    }

    static EmbedProperties BuildMovieEmbed(MovieDto movie)
    {
        var fields = new List<EmbedFieldProperties>();

        if (movie.ImdbRating != null)
        {
            fields.Add(new EmbedFieldProperties() { Name = "IMDB Rating", Value = $"{movie.ImdbRating}" });
        }

        if (movie.TmdbRating != null)
        {
            fields.Add(new EmbedFieldProperties() { Name = "TMDB Rating", Value = $"{movie.TmdbRating}" });
        }

        if (movie.RottenTomatoesRating != null)
        {
            fields.Add(new EmbedFieldProperties() { Name = "Rotten Tomatoes Rating", Value = $"{movie.RottenTomatoesRating}" });
        }

        if (movie.MetacriticRating != null)
        {
            fields.Add(new EmbedFieldProperties() { Name = "Metacritic Rating", Value = $"{movie.MetacriticRating}" });
        }

        var embed = new EmbedProperties()
            .WithTitle($"{movie.Title} ({movie.Year})")
            .WithDescription(movie.Overview)
            .WithImage(new EmbedImageProperties(movie.RemotePoster));

        if (fields.Count > 0)
        {
            embed.AddFields(fields);
        }

        return embed;
    }

    static EmbedProperties BuildSeriesEmbed(SeriesDto series)
    {
        var fields = new List<EmbedFieldProperties>();

        if (series.Rating != null)
        {
            fields.Add(new EmbedFieldProperties() { Name = "Rating", Value = $"{series.Rating}" });
        }

        var embed = new EmbedProperties()
            .WithTitle($"{series.Title} ({series.Year})")
            .WithDescription(series.Overview)
            .WithImage(new EmbedImageProperties(series.RemotePoster));

        if (fields.Count > 0)
        {
            embed.AddFields(fields);
        }

        return embed;
    }
}

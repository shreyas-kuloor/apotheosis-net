﻿using Apotheosis.Core.Features.MediaRequest.Interfaces;
using Microsoft.Extensions.Logging;

namespace Apotheosis.Server.Features.MediaRequest.Modules;
public sealed class RequestButtonModule(
    IActiveMediaSelectionCache activeMediaSelectionCache, 
    IMediaRequestMessageCache mediaRequestMessageCache,
    IMediaRequestService mediaRequestService,
    ILogger<RequestButtonModule> logger) : ComponentInteractionModule<ButtonInteractionContext>
{
    [ComponentInteraction("req-movie-button")]
    public async Task RequestMovieAsync()
    {
        await RespondAsync(InteractionCallback.DeferredMessage(MessageFlags.Ephemeral));

        var movieToRequest = activeMediaSelectionCache.GetActiveMovieSelection(Context.Message.Id);
        var messageIdsToDelete = mediaRequestMessageCache.GetAssociatedMediaRequestMessages(Context.Message.Id);
        mediaRequestMessageCache.ClearAssociatedMediaRequestMessages(Context.Message.Id);

        foreach (var messageId in messageIdsToDelete)
            await DeleteFollowupAsync(messageId);

        if (movieToRequest == null)
        {
            await FollowupAsync(
                new InteractionMessageProperties()
                .WithContent("Something went wrong when requesting the movie.")
                .WithFlags(MessageFlags.Ephemeral));
            return;
        }

        try
        {
            await mediaRequestService.RequestMovie(movieToRequest.Title!, movieToRequest.TmdbId);
        }
        catch (Exception)
        {
            await FollowupAsync(
                new InteractionMessageProperties()
                .WithContent("Something went wrong when requesting the movie.")
                .WithFlags(MessageFlags.Ephemeral));
            return;
        }

        logger.LogInformation("{User} requested {Title} ({Year})", Context.User.GlobalName, movieToRequest.Title, movieToRequest.Year);

        await FollowupAsync(
            new InteractionMessageProperties()
            .WithContent($"Requesting {movieToRequest.Title} ({movieToRequest.Year}).")
            .WithFlags(MessageFlags.Ephemeral));
    }

    [ComponentInteraction("req-series-button")]
    public async Task RequestSeriesAsync()
    {
        await RespondAsync(InteractionCallback.DeferredMessage(MessageFlags.Ephemeral));

        var seriesToRequest = activeMediaSelectionCache.GetActiveSeriesSelection(Context.Message.Id);
        var messageIdsToDelete = mediaRequestMessageCache.GetAssociatedMediaRequestMessages(Context.Message.Id);
        mediaRequestMessageCache.ClearAssociatedMediaRequestMessages(Context.Message.Id);

        foreach (var messageId in messageIdsToDelete)
            await DeleteFollowupAsync(messageId);

        if (seriesToRequest == null)
        {
            await FollowupAsync(
                new InteractionMessageProperties()
                .WithContent("Something went wrong when requesting the series.")
                .WithFlags(MessageFlags.Ephemeral));
            return;
        }

        try
        {
            await mediaRequestService.RequestSeries(seriesToRequest.Title, seriesToRequest.TvdbId);
        }
        catch (Exception)
        {
            await FollowupAsync(
                new InteractionMessageProperties()
                .WithContent("Something went wrong when requesting the series.")
                .WithFlags(MessageFlags.Ephemeral));
            return;
        }
        
        logger.LogInformation("{User} requested {Title} ({Year})", Context.User.GlobalName, seriesToRequest.Title, seriesToRequest.Year);

        await FollowupAsync(
            new InteractionMessageProperties()
            .WithContent($"Requesting {seriesToRequest.Title} ({seriesToRequest.Year}).")
            .WithFlags(MessageFlags.Ephemeral));
    }
}

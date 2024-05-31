using System.Collections.Concurrent;
using Apotheosis.Core.Features.AiChat.Exceptions;
using Apotheosis.Core.Features.DateTime.Interfaces;
using Apotheosis.Core.Features.MediaRequest.Configuration;
using Apotheosis.Core.Features.MediaRequest.Exceptions;
using Apotheosis.Core.Features.MediaRequest.Interfaces;
using Apotheosis.Core.Features.MediaRequest.Models;

namespace Apotheosis.Core.Features.MediaRequest.Services;

[Singleton]
public sealed class ActiveMediaSelectionCache(IDateTimeService dateTimeService, IOptions<MediaRequestSettings> mediaRequestOptions) : IActiveMediaSelectionCache
{
    readonly MediaRequestSettings mediaRequestSettings = mediaRequestOptions.Value;
    readonly ConcurrentDictionary<ulong, MovieCacheItem> movieCache = [];
    readonly ConcurrentDictionary<ulong, SeriesCacheItem> seriesCache = [];

    public MovieDto? GetActiveMovieSelection(ulong messageId)
    {
        ClearExpiredMovies();

        if (movieCache.TryGetValue(messageId, out var movie))
        {
            return movie.Item;
        }
        return null;
    }

    public SeriesDto? GetActiveSeriesSelection(ulong messageId)
    {
        ClearExpiredSeries();

        if (seriesCache.TryGetValue(messageId, out var series))
        {
            return series.Item;
        }
        return null;
    }

    public void StoreActiveMovieSelection(ulong messageId, MovieDto movie)
    {
        ClearExpiredMovies();

        var cacheItem = new MovieCacheItem { Item = movie, ExpiresAt = dateTimeService.UtcNow.AddMinutes(mediaRequestSettings.CacheExpirationMinutes) };

        if (!movieCache.TryAdd(messageId, cacheItem))
        {
            throw new MediaRequestCacheException();
        }
    }

    public void StoreActiveSeriesSelection(ulong messageId, SeriesDto series)
    {
        ClearExpiredSeries();

        var cacheItem = new SeriesCacheItem { Item = series, ExpiresAt = dateTimeService.UtcNow.AddMinutes(mediaRequestSettings.CacheExpirationMinutes) };

        if (!seriesCache.TryAdd(messageId, cacheItem))
        {
            throw new MediaRequestCacheException();
        }
    }

    void ClearExpiredMovies()
    {
        var nowDateTimeOffset = dateTimeService.UtcNow;
        var expiredMovies = movieCache.Where(m => m.Value.ExpiresAt <= nowDateTimeOffset);

        var successes = expiredMovies.Select(movieCache.TryRemove);

        if (successes.Any(s => !s))
        {
            throw new MediaRequestCacheException("An error occurred while trying to remove expired movies.");
        }
    }

    void ClearExpiredSeries()
    {
        var nowDateTimeOffset = dateTimeService.UtcNow;
        var expiredSeries = seriesCache.Where(s => s.Value.ExpiresAt <= nowDateTimeOffset);

        var successes = expiredSeries.Select(seriesCache.TryRemove);

        if (successes.Any(s => !s))
        {
            throw new MediaRequestCacheException("An error occurred while trying to remove expired series.");
        }
    }
}

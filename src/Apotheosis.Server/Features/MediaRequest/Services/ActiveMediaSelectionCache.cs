using System.Collections.Concurrent;
using Apotheosis.Core.Features.DateTime.Interfaces;
using Apotheosis.Core.Features.MediaRequest.Interfaces;
using Apotheosis.Core.Features.MediaRequest.Models;
using Apotheosis.Server.Features.MediaRequest.Configuration;
using Apotheosis.Server.Features.MediaRequest.Exceptions;

namespace Apotheosis.Server.Features.MediaRequest.Services;

[Singleton]
public sealed class ActiveMediaSelectionCache(IDateTimeService dateTimeService, IOptions<MediaRequestSettings> mediaRequestOptions) : IActiveMediaSelectionCache
{
    readonly MediaRequestSettings _mediaRequestSettings = mediaRequestOptions.Value;
    readonly ConcurrentDictionary<ulong, MovieCacheItem> _movieCache = [];
    readonly ConcurrentDictionary<ulong, SeriesCacheItem> _seriesCache = [];

    public MovieDto? GetActiveMovieSelection(ulong messageId)
    {
        ClearExpiredMovies();

        if (_movieCache.TryGetValue(messageId, out var movie))
        {
            return movie.Item;
        }
        return null;
    }

    public SeriesDto? GetActiveSeriesSelection(ulong messageId)
    {
        ClearExpiredSeries();

        if (_seriesCache.TryGetValue(messageId, out var series))
        {
            return series.Item;
        }
        return null;
    }

    public void StoreActiveMovieSelection(ulong messageId, MovieDto movie)
    {
        ClearExpiredMovies();

        var cacheItem = new MovieCacheItem { Item = movie, ExpiresAt = dateTimeService.UtcNow.AddMinutes(_mediaRequestSettings.CacheExpirationMinutes) };

        if (!_movieCache.TryAdd(messageId, cacheItem))
        {
            throw new MediaRequestCacheException();
        }
    }

    public void StoreActiveSeriesSelection(ulong messageId, SeriesDto series)
    {
        ClearExpiredSeries();

        var cacheItem = new SeriesCacheItem { Item = series, ExpiresAt = dateTimeService.UtcNow.AddMinutes(_mediaRequestSettings.CacheExpirationMinutes) };

        if (!_seriesCache.TryAdd(messageId, cacheItem))
        {
            throw new MediaRequestCacheException();
        }
    }

    void ClearExpiredMovies()
    {
        var nowDateTimeOffset = dateTimeService.UtcNow;
        var expiredMovies = _movieCache.Where(m => m.Value.ExpiresAt <= nowDateTimeOffset);

        var successes = expiredMovies.Select(_movieCache.TryRemove);

        if (successes.Any(s => !s))
        {
            throw new MediaRequestCacheException("An error occurred while trying to remove expired movies.");
        }
    }

    void ClearExpiredSeries()
    {
        var nowDateTimeOffset = dateTimeService.UtcNow;
        var expiredSeries = _seriesCache.Where(s => s.Value.ExpiresAt <= nowDateTimeOffset);

        var successes = expiredSeries.Select(_seriesCache.TryRemove);

        if (successes.Any(s => !s))
        {
            throw new MediaRequestCacheException("An error occurred while trying to remove expired series.");
        }
    }
}

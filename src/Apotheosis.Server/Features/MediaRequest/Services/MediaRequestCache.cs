using Apotheosis.Core.Features.DateTime.Interfaces;
using Apotheosis.Core.Features.MediaRequest.Interfaces;
using Apotheosis.Core.Features.MediaRequest.Models;
using Apotheosis.Server.Features.MediaRequest.Configuration;

namespace Apotheosis.Server.Features.MediaRequest.Services;

[Singleton]
public sealed class MediaRequestCache(IDateTimeService dateTimeService, IOptions<MediaRequestSettings> mediaRequestOptions) : IMediaRequestCache
{
    readonly MediaRequestSettings _mediaRequestSettings = mediaRequestOptions.Value;
    readonly List<MovieCacheItem> _movieCache = [];
    readonly List<SeriesCacheItem> _seriesCache = [];

    public MovieDto? GetCachedMovie(int tmdbId)
    {
        ClearCaches();

        return _movieCache.FirstOrDefault(m => m.Item.TmdbId == tmdbId)?.Item;
    }

    public SeriesDto? GetCachedSeries(int tvdbId)
    {
        ClearCaches();

        return _seriesCache.FirstOrDefault(s => s.Item.TvdbId == tvdbId)?.Item;
    }

    public void StoreMovieRequests(IEnumerable<MovieDto> movieResults)
    {
        ClearCaches();
        var currentDateTime = dateTimeService.UtcNow;

        var cacheItems = movieResults.Select(m => new MovieCacheItem { Item = m, ExpiresAt = currentDateTime.AddMinutes(_mediaRequestSettings.CacheExpirationMinutes) });
        _movieCache.AddRange(cacheItems);
    }
     
    public void StoreSeriesRequests(IEnumerable<SeriesDto> seriesResults)
    {
        ClearCaches();
        var currentDateTime = dateTimeService.UtcNow;

        var cacheItems = seriesResults.Select(s => new SeriesCacheItem { Item= s, ExpiresAt = currentDateTime.AddMinutes(_mediaRequestSettings.CacheExpirationMinutes) });
        _seriesCache.AddRange(cacheItems);
    }

    void ClearCaches()
    {
        var currentDateTime = dateTimeService.UtcNow;
        _movieCache.RemoveAll(m => m.ExpiresAt <= currentDateTime);
        _seriesCache.RemoveAll(m => m.ExpiresAt <= currentDateTime);
    }
}

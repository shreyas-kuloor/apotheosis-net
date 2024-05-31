using Apotheosis.Core.Features.DateTime.Interfaces;
using Apotheosis.Core.Features.MediaRequest.Configuration;
using Apotheosis.Core.Features.MediaRequest.Interfaces;
using Apotheosis.Core.Features.MediaRequest.Models;

namespace Apotheosis.Core.Features.MediaRequest.Services;

[Singleton]
public sealed class MediaRequestCache(IDateTimeService dateTimeService, IOptions<MediaRequestSettings> mediaRequestOptions) : IMediaRequestCache
{
    readonly MediaRequestSettings mediaRequestSettings = mediaRequestOptions.Value;
    readonly List<MovieCacheItem> movieCache = [];
    readonly List<SeriesCacheItem> seriesCache = [];

    public MovieDto? GetCachedMovie(int tmdbId)
    {
        ClearCaches();

        return movieCache.FirstOrDefault(m => m.Item.TmdbId == tmdbId)?.Item;
    }

    public SeriesDto? GetCachedSeries(int tvdbId)
    {
        ClearCaches();

        return seriesCache.FirstOrDefault(s => s.Item.TvdbId == tvdbId)?.Item;
    }

    public void StoreMovieRequests(IEnumerable<MovieDto> movieResults)
    {
        ClearCaches();
        var currentDateTime = dateTimeService.UtcNow;

        var cacheItems = movieResults.Select(m => new MovieCacheItem { Item = m, ExpiresAt = currentDateTime.AddMinutes(mediaRequestSettings.CacheExpirationMinutes) });
        movieCache.AddRange(cacheItems);
    }
     
    public void StoreSeriesRequests(IEnumerable<SeriesDto> seriesResults)
    {
        ClearCaches();
        var currentDateTime = dateTimeService.UtcNow;

        var cacheItems = seriesResults.Select(s => new SeriesCacheItem { Item= s, ExpiresAt = currentDateTime.AddMinutes(mediaRequestSettings.CacheExpirationMinutes) });
        seriesCache.AddRange(cacheItems);
    }

    void ClearCaches()
    {
        var currentDateTime = dateTimeService.UtcNow;
        movieCache.RemoveAll(m => m.ExpiresAt <= currentDateTime);
        seriesCache.RemoveAll(m => m.ExpiresAt <= currentDateTime);
    }
}

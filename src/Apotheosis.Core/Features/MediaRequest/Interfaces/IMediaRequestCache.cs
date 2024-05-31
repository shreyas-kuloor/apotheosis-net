using Apotheosis.Core.Features.MediaRequest.Models;

namespace Apotheosis.Core.Features.MediaRequest.Interfaces;

public interface IMediaRequestCache
{
    MovieDto? GetCachedMovie(int tmdbId);
    SeriesDto? GetCachedSeries(int tvdbId);
    void StoreMovieRequests(IEnumerable<MovieDto> movieResults);
    void StoreSeriesRequests(IEnumerable<SeriesDto> seriesResults);
    
}

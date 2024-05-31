using Apotheosis.Core.Features.MediaRequest.Models;

namespace Apotheosis.Core.Features.MediaRequest.Interfaces;

public interface IMediaRequestService
{
    Task<IEnumerable<MovieDto>> SearchMovies(string searchTerm);

    Task<IEnumerable<SeriesDto>> SearchSeries(string searchTerm);

    Task RequestMovie(string title, int tmdbId);

    Task RequestSeries(string title, int tvdbId);
}

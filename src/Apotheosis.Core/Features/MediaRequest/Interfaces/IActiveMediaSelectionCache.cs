using Apotheosis.Core.Features.MediaRequest.Models;

namespace Apotheosis.Core.Features.MediaRequest.Interfaces;

public interface IActiveMediaSelectionCache
{
    MovieDto? GetActiveMovieSelection(ulong messageId);
    SeriesDto? GetActiveSeriesSelection(ulong messageId);
    void StoreActiveMovieSelection(ulong messageId, MovieDto movie);
    void StoreActiveSeriesSelection(ulong messageId, SeriesDto series);
}

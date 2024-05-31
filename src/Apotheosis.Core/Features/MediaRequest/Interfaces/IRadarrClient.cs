using Apotheosis.Core.Features.MediaRequest.Models.Radarr.Request;
using Apotheosis.Core.Features.MediaRequest.Models.Radarr.Response;
using Apotheosis.Core.Features.MediaRequest.Models.Shared;

namespace Apotheosis.Core.Features.MediaRequest.Interfaces;

public interface IRadarrClient
{
    Task<IEnumerable<Movie>> GetMovieByTermAsync(string term);
    Task<IEnumerable<RootFolder>> GetRootFoldersAsync();
    Task<IEnumerable<QualityProfile>> GetQualityProfilesAsync();
    Task RequestMovieAsync(MovieRequest request);
}
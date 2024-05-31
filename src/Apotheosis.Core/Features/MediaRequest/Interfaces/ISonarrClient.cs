using Apotheosis.Core.Features.MediaRequest.Models.Radarr.Request;
using Apotheosis.Core.Features.MediaRequest.Models.Shared;
using Apotheosis.Core.Features.MediaRequest.Models.Sonarr.Response;

namespace Apotheosis.Core.Features.MediaRequest.Interfaces;

public interface ISonarrClient
{
    Task<IEnumerable<Series>> GetSeriesByTermAsync(string term);
    Task<IEnumerable<RootFolder>> GetRootFoldersAsync();
    Task<IEnumerable<QualityProfile>> GetQualityProfilesAsync();
    Task RequestSeriesAsync(SeriesRequest request);
}
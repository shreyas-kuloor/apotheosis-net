using Apotheosis.Core.Features.MediaRequest.Configuration;
using Apotheosis.Core.Features.MediaRequest.Interfaces;
using Apotheosis.Core.Features.MediaRequest.Models;
using Apotheosis.Core.Features.MediaRequest.Models.Radarr.Request;

namespace Apotheosis.Core.Features.MediaRequest.Services;

[Scoped]
public sealed class MediaRequestService(
    IMediaRequestCache mediaRequestCache, 
    IRadarrClient radarrClient, 
    ISonarrClient sonarrClient, 
    IOptions<MediaRequestSettings> mediaRequestOptions) : IMediaRequestService
{
    readonly MediaRequestSettings mediaRequestSettings = mediaRequestOptions.Value;

    public async Task<IEnumerable<MovieDto>> SearchMovies(string searchTerm)
    {
        var movies = await radarrClient.GetMovieByTermAsync(searchTerm);

        var movieDtos = movies.Take(mediaRequestSettings.OptionCount).Select(m =>
        {
            MediaStatus status;
            if (m.IsAvailable && m.MovieFileId > 0 && m.Monitored)
                status = MediaStatus.Available;
            else if (m.IsAvailable && m.MovieFileId == 0 && m.Monitored)
                status = MediaStatus.Processing;
            else
                status = MediaStatus.NotAdded;

            return new MovieDto
            {
                TmdbId = m.TmdbId,
                Title = m.Title,
                RemotePoster = m.RemotePoster,
                Year = m.Year,
                Overview = m.Overview,
                Status = status,
                ImdbRating = m.Ratings?.Imdb?.Value,
                TmdbRating = m.Ratings?.Tmdb?.Value,
                MetacriticRating = m.Ratings?.Metacritic?.Value,
                RottenTomatoesRating = m.Ratings?.RottenTomatoes?.Value,
            };
        });

        mediaRequestCache.StoreMovieRequests(movieDtos);

        return movieDtos;
    }

    public async Task<IEnumerable<SeriesDto>> SearchSeries(string searchTerm)
    {
        var series = await sonarrClient.GetSeriesByTermAsync(searchTerm);

        var seriesDtos = series.Take(mediaRequestSettings.OptionCount).Select(s => {
            MediaStatus status;
            if (s.IsAvailable && s.HasFile && s.Monitored)
                status = MediaStatus.Available;
            else if (!s.IsAvailable && s.HasFile && s.Monitored)
                status = MediaStatus.Processing;
            else
                status = MediaStatus.NotAdded;

            return new SeriesDto
            {
                TvdbId = s.TvdbId,
                Title = s.Title,
                RemotePoster = s.RemotePoster,
                Year = s.Year,
                Overview = s.Overview,
                Status = status,
                Rating = s.Ratings?.Value,
            };
        });

        mediaRequestCache.StoreSeriesRequests(seriesDtos);

        return seriesDtos;
    }

    public async Task RequestMovie(string title, int tmdbId)
    {
        var rootFolders = await radarrClient.GetRootFoldersAsync();
        var qualityProfiles = await radarrClient.GetQualityProfilesAsync();

        var request = new MovieRequest
        {
            Title = title,
            TmdbId = tmdbId,
            RootFolderPath = rootFolders.FirstOrDefault()?.Path,
            Monitored = true,
            QualityProfileId = qualityProfiles.FirstOrDefault(q => q.Name == mediaRequestSettings.QualityProfileName)?.Id ?? 4,
            AddOptions = new()
            {
                SearchForMovie = true,
            },
        };

        await radarrClient.RequestMovieAsync(request);
    }

    public async Task RequestSeries(string title, int tvdbId)
    {
        var rootFolders = await radarrClient.GetRootFoldersAsync();
        var qualityProfiles = await radarrClient.GetQualityProfilesAsync();

        var request = new SeriesRequest
        {
            Title = title,
            TvdbId = tvdbId,
            RootFolderPath = rootFolders.FirstOrDefault()?.Path,
            Monitored = true,
            QualityProfileId = qualityProfiles.FirstOrDefault(q => q.Name == mediaRequestSettings.QualityProfileName)?.Id ?? 4,
            AddOptions = new()
            {
                SearchForMovie = true,
            },
        };

        await sonarrClient.RequestSeriesAsync(request);
    }
}

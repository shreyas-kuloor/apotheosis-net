namespace Apotheosis.Core.Features.MediaRequest.Models;

public sealed class MovieDto
{
    public int TmdbId { get; set; }

    public string? Title { get; set; }

    public string? RemotePoster { get; set; }

    public int Year { get; set; }

    public string? Overview { get; set; }

    public MediaStatus Status { get; set; }

    public double? ImdbRating { get; set; }

    public double? TmdbRating { get; set; }

    public double? MetacriticRating { get; set; }

    public double? RottenTomatoesRating { get; set; }
}

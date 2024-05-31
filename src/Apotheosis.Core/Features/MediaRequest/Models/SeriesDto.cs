namespace Apotheosis.Core.Features.MediaRequest.Models;

public sealed class SeriesDto
{
    public int TvdbId { get; set; }

    public string Title { get; set; } = default!;

    public string? RemotePoster { get; set; }

    public int Year { get; set; }

    public string? Overview { get; set; }

    public MediaStatus Status { get; set; }

    public double? Rating { get; set; }
}

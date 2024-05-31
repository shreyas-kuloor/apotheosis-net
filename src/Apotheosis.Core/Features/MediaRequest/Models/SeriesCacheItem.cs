namespace Apotheosis.Core.Features.MediaRequest.Models;
internal class SeriesCacheItem
{
    public SeriesDto Item { get; set; } = default!;

    public DateTimeOffset ExpiresAt { get; set; }
}

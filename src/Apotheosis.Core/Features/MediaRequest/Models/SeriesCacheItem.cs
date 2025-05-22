namespace Apotheosis.Core.Features.MediaRequest.Models;
public class SeriesCacheItem
{
    public SeriesDto Item { get; set; } = default!;

    public DateTimeOffset ExpiresAt { get; set; }
}

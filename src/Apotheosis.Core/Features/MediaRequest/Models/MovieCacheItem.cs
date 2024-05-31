namespace Apotheosis.Core.Features.MediaRequest.Models;
public sealed class MovieCacheItem
{
    public MovieDto Item { get; set; } = default!;

    public DateTimeOffset ExpiresAt { get; set; }
}

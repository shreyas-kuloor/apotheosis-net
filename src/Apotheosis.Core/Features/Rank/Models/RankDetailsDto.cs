using Apotheosis.Core.Features.Rank.SmartEnums;

namespace Apotheosis.Core.Features.Rank.Models;
public record RankDetailsDto

{
    public QueueType QueueType { get; set; } = default!;

    public string? Tier { get; set; }

    public string? Rank { get; set; }

    public int LeaguePoints { get; set; }

    public int Wins { get; set; }

    public int Losses { get; set; }
}

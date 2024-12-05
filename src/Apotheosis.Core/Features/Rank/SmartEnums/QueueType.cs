using Ardalis.SmartEnum;
using Newtonsoft.Json;

namespace Apotheosis.Core.Features.Rank.SmartEnums;
public sealed class QueueType : SmartEnum<QueueType, string>
{
    public static readonly QueueType RankedSoloDuo = new(nameof(RankedSoloDuo), "RANKED_SOLO_5x5");
    public static readonly QueueType Unknown = new(nameof(Unknown), "UNKNOWN");

    [JsonConstructor]
    public QueueType(string name, string value) : base(name, value)
    {
    }
}

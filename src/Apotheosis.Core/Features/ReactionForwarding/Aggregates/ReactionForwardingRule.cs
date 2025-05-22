namespace Apotheosis.Core.Features.ReactionForwarding.Aggregates;

public sealed class ReactionForwardingRule
{
    public int Id { get; set; }
    public ulong? EmojiId { get; set; }
    public string? Name { get; set; }
    public ulong ChannelId { get; set; }
}
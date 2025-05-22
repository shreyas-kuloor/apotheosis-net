namespace Apotheosis.Core.Features.ReactionForwarding.Aggregates;

public sealed class ForwardedMessage
{
    public int Id { get; set; }
    public ulong MessageId { get; set; }
    public ulong ChannelId { get; set; }
}
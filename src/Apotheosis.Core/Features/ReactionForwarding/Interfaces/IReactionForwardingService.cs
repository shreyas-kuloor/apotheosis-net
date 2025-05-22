namespace Apotheosis.Core.Features.ReactionForwarding.Interfaces;

public interface IReactionForwardingService
{
    Task AddReactionForwardingRuleAsync(string emoji, string channel);
    
    Task RemoveReactionForwardingRuleAsync(string emoji, string channel);
    
    Task<List<ReactionForwardingRule>> GetReactionForwardingRulesAsync();
    
    Task<List<ReactionForwardingRule>> GetReactionForwardingRulesByEmojiAsync(ulong? emojiId, string? emojiName);
    
    Task<bool> IsMessageAlreadyForwardedAsync(ulong messageId, ulong channelId);
    
    Task SaveForwardedMessageAsync(ulong messageId, ulong channelId);
}
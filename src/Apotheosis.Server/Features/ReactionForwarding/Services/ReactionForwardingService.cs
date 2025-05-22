using Emoji = GEmojiSharp.Emoji;

namespace Apotheosis.Server.Features.ReactionForwarding.Services;

[Scoped]
public sealed partial class ReactionForwardingService(ApotheosisDbContext apotheosisDbContext) : IReactionForwardingService
{
    [GeneratedRegex(@"<:([^:]+):(\d+)>$")]
    private static partial Regex DiscordEmojiPatternRegex();

    [GeneratedRegex(@"^<#(\d+)>$")]
    private static partial Regex ChannelPatternRegex();
    
    [GeneratedRegex(Emoji.RegexPattern)]
    private static partial Regex EmojiPatternRegex();
    
    public async Task AddReactionForwardingRuleAsync(string emoji, string channel)
    {
        var (emojiName, emojiId, channelId) = GetEmojiAndChannelDetails(emoji, channel);
        
        var reactionForwardingRule = new ReactionForwardingRule
        {
            EmojiId = emojiId,
            Name = emojiName,
            ChannelId = channelId,
        };
        
        await apotheosisDbContext.ReactionForwardingRules.AddAsync(reactionForwardingRule);
        await apotheosisDbContext.SaveChangesAsync();
    }

    public async Task RemoveReactionForwardingRuleAsync(string emoji, string channel)
    {
        var (emojiName, emojiId, channelId) = GetEmojiAndChannelDetails(emoji, channel);
        var reactionForwardingRule = await apotheosisDbContext.ReactionForwardingRules.FirstOrDefaultAsync(
            rfr => rfr.EmojiId == emojiId 
            && rfr.ChannelId == channelId 
            && rfr.Name == emojiName);

        if (reactionForwardingRule == null)
        {
            throw new Exception();
        }
        
        apotheosisDbContext.ReactionForwardingRules.Remove(reactionForwardingRule);
        await apotheosisDbContext.SaveChangesAsync();
    }

    public async Task<List<ReactionForwardingRule>> GetReactionForwardingRulesAsync()
    {
        return await apotheosisDbContext.ReactionForwardingRules.ToListAsync();
    }
    
    public async Task<List<ReactionForwardingRule>> GetReactionForwardingRulesByEmojiAsync(ulong? emojiId, string? emojiName)
    {
        return await apotheosisDbContext.ReactionForwardingRules.Where(rfr => rfr.EmojiId == emojiId && rfr.Name == emojiName).ToListAsync();
    }

    public async Task<bool> IsMessageAlreadyForwardedAsync(ulong messageId, ulong channelId)
    {
        var forwardedMessage = await apotheosisDbContext.ForwardedMessages.FirstOrDefaultAsync(fm => fm.MessageId == messageId && fm.ChannelId == channelId);
        
        return forwardedMessage != null;
    }

    public async Task SaveForwardedMessageAsync(ulong messageId, ulong channelId)
    {
        var forwardedMessage = new ForwardedMessage
        {
            MessageId = messageId,
            ChannelId = channelId,
        };
        
        await apotheosisDbContext.ForwardedMessages.AddAsync(forwardedMessage);
        await apotheosisDbContext.SaveChangesAsync();
    }

    static (string? EmojiName, ulong? EmojiId, ulong ChannelId) GetEmojiAndChannelDetails(string emoji, string channel)
    {
        var discordEmojiRegex = DiscordEmojiPatternRegex();
        var channelRegex = ChannelPatternRegex();
        var emojiRegex = EmojiPatternRegex();

        var successfulDiscordEmojiMatches = discordEmojiRegex.Matches(emoji).Where(m => m.Success).ToList();
        var successfulChannelMatches = channelRegex.Matches(channel).Where(m => m.Success).ToList();
        var successfulEmojiMatches = emojiRegex.Matches(emoji).Where(m => m.Success).ToList();

        ulong? emojiId = null;
        string? emojiName;
        if (successfulDiscordEmojiMatches.Count != 0)
        {
            var emojiIds = successfulDiscordEmojiMatches
                .Select(m => 
                {
                    var name = m.Groups[1].Value;
                    var id = ulong.TryParse(m.Groups[2].Value, out var parsedId) ? parsedId : 0;
                    return (Name: name, Id: id);
                })
                .Where(e => e.Id != 0)
                .Distinct();

            var emojiDetails = emojiIds.FirstOrDefault();
            emojiId = emojiDetails.Id;
            emojiName = emojiDetails.Name;
        }
        else if (successfulEmojiMatches.Count != 0)
        {
            emojiName = successfulEmojiMatches.FirstOrDefault()?.Value;
        }
        else
        {
            throw new InvalidOperationException();
        }

        ulong channelId;
        if (successfulChannelMatches.Count != 0)
        {
            var channelIds = successfulChannelMatches
                .Select(m => ulong.TryParse(m.Groups[1].Value, out var parsedId) ? parsedId : 0)
                .Where(i => i != 0)
                .Distinct();
            
            channelId = channelIds.FirstOrDefault();
        }
        else
        {
            throw new InvalidOperationException();
        }
        
        return (emojiName, emojiId, channelId);
    }
}
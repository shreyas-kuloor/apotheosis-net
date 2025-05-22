namespace Apotheosis.Server.Features.ReactionForwarding.Modules;

[SlashCommand("forward", "Manage reaction forwarding rules.")]
public sealed class ReactionForwardingRuleModule(
    IReactionForwardingService reactionForwardingService,
    IOptions<FeatureFlagSettings> featureFlagOptions) : ApplicationCommandModule<SlashCommandContext>
{
    readonly FeatureFlagSettings _featureFlagSettings = featureFlagOptions.Value;
    
    [RequireUserPermissions<SlashCommandContext>(Permissions.Administrator)]
    [SubSlashCommand("add", "Create a reaction forwarding rule.")]
    public async Task AddReactionForwardingRuleAsync(string emoji, string channel)
    {
        if (!_featureFlagSettings.ReactionForwardingEnabled)
        {
            await RespondAsync(InteractionCallback.Message(
                new InteractionMessageProperties()
                    .WithContent("This command is currently disabled. Please try again later!")
                    .WithFlags(MessageFlags.Ephemeral)));
            return;
        }
        
        try
        {
            await reactionForwardingService.AddReactionForwardingRuleAsync(emoji, channel);
            
            await RespondAsync(InteractionCallback.Message(
                new InteractionMessageProperties()
                    .WithContent($"Created reaction forwarding rule: {emoji} to {channel}")
                    .WithFlags(MessageFlags.Ephemeral)));
        }
        catch (InvalidOperationException)
        {
            await RespondAsync(InteractionCallback.Message(
                new InteractionMessageProperties()
                    .WithContent("Invalid emoji or channel")
                    .WithFlags(MessageFlags.Ephemeral)));
        }
    }
    
    [RequireUserPermissions<SlashCommandContext>(Permissions.Administrator)]
    [SubSlashCommand("remove", "Remove a reaction forwarding rule.")]
    public async Task RemoveReactionForwardingRuleAsync(string emoji, string channel)
    {
        if (!_featureFlagSettings.ReactionForwardingEnabled)
        {
            await RespondAsync(InteractionCallback.Message(
                new InteractionMessageProperties()
                    .WithContent("This command is currently disabled. Please try again later!")
                    .WithFlags(MessageFlags.Ephemeral)));
            return;
        }
        
        try
        {
            await reactionForwardingService.RemoveReactionForwardingRuleAsync(emoji, channel);
            
            await RespondAsync(InteractionCallback.Message(
                new InteractionMessageProperties()
                    .WithContent($"Removed reaction forwarding rule: {emoji} to {channel}")
                    .WithFlags(MessageFlags.Ephemeral)));
        }
        catch (InvalidOperationException)
        {
            await RespondAsync(InteractionCallback.Message(
                new InteractionMessageProperties()
                    .WithContent("Invalid emoji or channel")
                    .WithFlags(MessageFlags.Ephemeral)));
        }
    }
    
    [SubSlashCommand("list", "List reaction forwarding rules.")]
    public async Task ListReactionForwardingRulesAsync()
    {
        if (!_featureFlagSettings.ReactionForwardingEnabled)
        {
            await RespondAsync(InteractionCallback.Message(
                new InteractionMessageProperties()
                    .WithContent("This command is currently disabled. Please try again later!")
                    .WithFlags(MessageFlags.Ephemeral)));
            return;
        }
        
        var reactionForwardingRules = await reactionForwardingService.GetReactionForwardingRulesAsync();
        var ruleStrings = reactionForwardingRules
            .Select(rfr => rfr.EmojiId.HasValue 
                ? $"- Emote: <:{rfr.Name}:{rfr.EmojiId}>, Channel: <#{rfr.ChannelId}>" 
                : $"- Emote: {rfr.Name}, Channel: <#{rfr.ChannelId}>");
        
        await RespondAsync(InteractionCallback.Message(
            new InteractionMessageProperties()
                .WithContent(string.Join('\n', ruleStrings))
                .WithFlags(MessageFlags.Ephemeral)));
    }
}
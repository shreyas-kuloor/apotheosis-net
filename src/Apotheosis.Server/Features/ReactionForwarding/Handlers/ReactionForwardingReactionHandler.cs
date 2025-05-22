namespace Apotheosis.Server.Features.ReactionForwarding.Handlers;

[GatewayEvent(nameof(GatewayClient.MessageReactionAdd))]
public sealed class ReactionForwardingReactionHandler(
    RestClient discordClient,
    IReactionForwardingService reactionForwardingService,
    IOptions<FeatureFlagSettings> featureFlagOptions)
    : IGatewayEventHandler<MessageReactionAddEventArgs>
{
    readonly FeatureFlagSettings _featureFlagSettings = featureFlagOptions.Value;
    
    public async ValueTask HandleAsync(MessageReactionAddEventArgs args)
    {
        if (!_featureFlagSettings.ReactionForwardingEnabled)
            return;

        var reactionForwardingRules = await reactionForwardingService.GetReactionForwardingRulesByEmojiAsync(args.Emoji.Id, args.Emoji.Name);

        foreach (var reactionForwardingRule in reactionForwardingRules)
        {
            if (await reactionForwardingService.IsMessageAlreadyForwardedAsync(args.MessageId, reactionForwardingRule.ChannelId))
            {
                continue;
            }
            
            var forwardProperties = MessageReferenceProperties.Forward(args.ChannelId, args.MessageId);
            await discordClient.SendMessageAsync(reactionForwardingRule.ChannelId, new MessageProperties().WithMessageReference(forwardProperties));

            await reactionForwardingService.SaveForwardedMessageAsync(args.MessageId, reactionForwardingRule.ChannelId);
        }
    }
}

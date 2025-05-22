using Apotheosis.Core.Features.AiChat.Interfaces;
using Apotheosis.Server.Features.AiChat.Exceptions;
using Microsoft.Extensions.Logging;

namespace Apotheosis.Server.Features.AiChat.Modules;

public sealed class ChatModule(
    IAiChatService aiChatService,
    ILogger<ChatModule> logger,
    IOptions<FeatureFlagSettings> featureFlagOptions)
    : ApplicationCommandModule<SlashCommandContext>
{
    readonly FeatureFlagSettings _featureFlagSettings = featureFlagOptions.Value;

    [SlashCommand("chat", "Chat with the bot.")]
    public async Task ChatAsync(string prompt)
    {
        if (!_featureFlagSettings.ChatEnabled)
        {
            await RespondAsync(InteractionCallback.Message(
                new InteractionMessageProperties()
                .WithContent("This command is currently disabled. Please try again later!")
                .WithFlags(MessageFlags.Ephemeral)));
            return;
        }

        logger.LogInformation("{User} used /chat {Prompt}", Context.User.GlobalName, prompt);

        await RespondAsync(InteractionCallback.DeferredMessage());

        string response;
        try
        {
            response = await aiChatService.GetChatResponseAsync(prompt, CancellationToken.None);
        }
        catch (AiChatNetworkException)
        {
            await FollowupAsync(new InteractionMessageProperties
            {
                Content = "Sorry, an error occurred while trying to reach the AI service. Please try again later or contact an admin.",
                Flags = MessageFlags.Ephemeral
            });
            return;
        }
        
        if (string.IsNullOrEmpty(response))
        {
            await FollowupAsync(
                new InteractionMessageProperties()
                {
                    Content = "Sorry, an error occurred while trying to reach the AI service. Please try again later or contact an admin.",
                    Flags = MessageFlags.Ephemeral
                });

            return;
        }
        
        await FollowupAsync(
            new InteractionMessageProperties()
                .WithContent(response));
    }
}
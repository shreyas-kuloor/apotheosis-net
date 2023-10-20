using Apotheosis.Components.AiChat.Interfaces;
using Apotheosis.Components.Client.Services;
using Discord.Interactions;

namespace Apotheosis.Components.AiChat.Modules;

public class AiChatModule : InteractionModuleBase<SocketInteractionContext>
{
    public InteractionService? Commands { get; set; }
    private readonly IAiChatService _aiChatService;

    public AiChatModule(InteractionHandler interactionHandler, IAiChatService aiChatService)
    {
        _aiChatService = aiChatService;
    }

    [SlashCommand("chat", "Start a chat with Apotheosis")]
    public async Task SendAiChatAsync(string message)
    {
        var response = await _aiChatService.InitiateChatAndGetResponseAsync(message);

        await RespondAsync(text: response);
    }
}
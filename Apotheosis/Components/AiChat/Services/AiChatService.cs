using Apotheosis.Components.AiChat.Configuration;
using Apotheosis.Components.AiChat.Interfaces;
using Apotheosis.Components.AiChat.Models;
using Microsoft.Extensions.Options;

namespace Apotheosis.Components.AiChat.Services;

public class AiChatService : IAiChatService
{
    private readonly IAiChatNetworkDriver _aiChatNetworkDriver;
    private readonly AiChatSettings _aiChatSettings;

    public AiChatService(IAiChatNetworkDriver aiChatNetworkDriver, IOptions<AiChatSettings> aiChatOptions)
    {
        _aiChatNetworkDriver = aiChatNetworkDriver;
        _aiChatSettings = aiChatOptions.Value;
    }
    
    public async Task<string> InitiateChatAndGetResponseAsync(string message)
    {
        var request = new OpenAiChatRequest
        {
            Model = _aiChatSettings.OpenAiModel,
            Messages = new List<OpenAiChatMessageRequest>
            {
                new()
                {
                    MessageRole = OpenAiChatRole.System,
                    Content = _aiChatSettings.ChatSystemInstruction,
                },
                new()
                {
                    MessageRole = OpenAiChatRole.User,
                    Content = message,
                }
            },
        };
        
        var response = await _aiChatNetworkDriver.SendRequestAsync<OpenAiChatRequest, OpenAiChatResponse>("chat/completions", HttpMethod.Post, request);

        return string.IsNullOrWhiteSpace(response?.Choices?.FirstOrDefault()?.Message?.Content) 
            ? "Something went wrong when I was thinking up a response. Please reach out to a server admin for help!" 
            : response.Choices.FirstOrDefault()!.Message!.Content!;
    }
}
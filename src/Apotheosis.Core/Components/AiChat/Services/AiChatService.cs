using Apotheosis.Core.Components.AiChat.Configuration;
using Apotheosis.Core.Components.AiChat.Interfaces;
using Apotheosis.Core.Components.AiChat.Models;

namespace Apotheosis.Core.Components.AiChat.Services;

public sealed class AiChatService(
    IAiChatNetworkDriver aiChatNetworkDriver,
    AiChatSettings aiChatSettings)
    : IAiChatService
{
    public async Task<IEnumerable<AiChatMessageDto>> SendSingleMessageToAiAsync(string prompt, string? systemMessage = null)
    {
        var request = new AiChatRequest
        {
            Model = aiChatSettings.OpenAiModel,
            Messages = new List<AiChatMessage>
            {
                new()
                {
                    Role = "system",
                    Content = string.IsNullOrWhiteSpace(systemMessage) ? aiChatSettings.ChatSystemInstruction : systemMessage
                },
                new()
                {
                    Role = "user",
                    Content = prompt
                }
            }
        };

        var response =
            await aiChatNetworkDriver.SendRequestAsync<AiChatResponse>("/v1/chat/completions", HttpMethod.Post, request);

        return response.Choices!.Select(c => new AiChatMessageDto
        {
            Index = c.Index,
            Content = c.Message!.Content
        });
    }

    public async Task<IEnumerable<AiChatMessageDto>> SendMultipleMessagesToAiAsync(IEnumerable<MessageDto> messages)
    {
        var transformedMessages = messages.Select(tm => new AiChatMessage
        {
            Role = tm.IsBot ? "assistant" : "user",
            Content = tm.Content,
        });
        
        var request = new AiChatRequest
        {
            Model = aiChatSettings.OpenAiModel,
            Messages = new List<AiChatMessage>
            {
                new()
                {
                    Role = "system",
                    Content = aiChatSettings.ChatSystemInstruction
                }
            }.Concat(transformedMessages).ToList()
        };

        var response =
            await aiChatNetworkDriver.SendRequestAsync<AiChatResponse>("/v1/chat/completions", HttpMethod.Post, request);

        return response.Choices!.Select(c => new AiChatMessageDto
        {
            Index = c.Index,
            Content = c.Message!.Content
        });
    }
}
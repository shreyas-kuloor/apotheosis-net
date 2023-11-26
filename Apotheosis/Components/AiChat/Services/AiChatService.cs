using Apotheosis.Components.AiChat.Configuration;
using Apotheosis.Components.AiChat.Interfaces;
using Apotheosis.Components.AiChat.Models;
using Microsoft.Extensions.Options;

namespace Apotheosis.Components.AiChat.Services;

public sealed class AiChatService : IAiChatService
{
    private readonly IAiChatNetworkDriver _aiChatNetworkDriver;
    private readonly AiChatSettings _aiChatSettings;

    public AiChatService(
        IAiChatNetworkDriver aiChatNetworkDriver,
        IOptions<AiChatSettings> aiChatOptions)
    {
        _aiChatNetworkDriver = aiChatNetworkDriver;
        _aiChatSettings = aiChatOptions.Value;
    }
    
    public async Task<IEnumerable<AiChatMessageDto>> InitializeThreadToAiAsync(string initialPrompt)
    {
        var request = new AiChatRequest
        {
            Model = _aiChatSettings.OpenAiModel,
            Messages = new List<AiChatMessage>
            {
                new()
                {
                    Role = "system",
                    Content = _aiChatSettings.ChatSystemInstruction
                },
                new()
                {
                    Role = "user",
                    Content = initialPrompt
                }
            }
        };

        var response =
            await _aiChatNetworkDriver.SendRequestAsync<AiChatResponse>("/v1/chat/completions", HttpMethod.Post, request);

        return response.Choices!.Select(c => new AiChatMessageDto
        {
            Index = c.Index,
            Content = c.Message!.Content
        });
    }

    public async Task<IEnumerable<AiChatMessageDto>> SendThreadToAiAsync(IEnumerable<ThreadMessageDto> threadMessages)
    {
        var transformedMessages = threadMessages.Select(tm => new AiChatMessage
        {
            Role = tm.IsBot ? "assistant" : "user",
            Content = tm.Content,
        });
        
        var request = new AiChatRequest
        {
            Model = _aiChatSettings.OpenAiModel,
            Messages = new List<AiChatMessage>
            {
                new()
                {
                    Role = "system",
                    Content = _aiChatSettings.ChatSystemInstruction
                }
            }.Concat(transformedMessages).ToList()
        };

        var response =
            await _aiChatNetworkDriver.SendRequestAsync<AiChatResponse>("/v1/chat/completions", HttpMethod.Post, request);

        return response.Choices!.Select(c => new AiChatMessageDto
        {
            Index = c.Index,
            Content = c.Message!.Content
        });
    }
}
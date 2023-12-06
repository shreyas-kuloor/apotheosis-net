using Apotheosis.Core.Components.AiChat.Configuration;
using Apotheosis.Core.Components.AiChat.Interfaces;
using Apotheosis.Core.Components.AiChat.Models;
using Apotheosis.Core.Components.AiChat.Services;
using Apotheosis.Test.Utils;
using FluentAssertions;
using Moq;

namespace Apotheosis.Test.Unit.Components.AiChat.Services;

public sealed class AiChatServiceTests : IDisposable
{
    private readonly Mock<IAiChatNetworkDriver> _aiChatNetworkDriverMock;
    private readonly AiChatService _aiChatService;
    
    private readonly AiChatSettings _aiChatSettings = new()
    {
        OpenAiModel = "model",
        ChatSystemInstruction = "example system instruction"
    };

    public AiChatServiceTests()
    {
        _aiChatNetworkDriverMock = new Mock<IAiChatNetworkDriver>(MockBehavior.Strict);

        _aiChatService = new AiChatService(_aiChatNetworkDriverMock.Object, _aiChatSettings);
    }

    public void Dispose()
    {
        _aiChatNetworkDriverMock.VerifyAll();
    }

    [Fact]
    public async Task SendSingleMessageToAiAsync_CallsNetworkDriverAndReturnsResponseDtos_GivenInitialPrompt()
    {
        const string initialPrompt = "example prompt";
        const string responseMessageContent = "example response";
        
        var expectedRequest = new AiChatRequest
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

        var response = new AiChatResponse
        {
            Id = "abc",
            Object = "completion",
            Created = 1700885588,
            Choices = new List<AiChatChoice>
            {
                new()
                {
                    Index = 0,
                    FinishReason = "complete",
                    Message = new AiChatMessage
                    {
                        Role = "assistant",
                        Content = responseMessageContent
                    }
                }
            },
            Usage = new AiChatUsage
            {
                CompletionTokens = 10,
                PromptTokens = 10,
                TotalTokens = 20
            }
        };

        var expectedDtos = new List<AiChatMessageDto>
        {
            new()
            {
                Index = 0,
                Content = responseMessageContent
            }
        };

        _aiChatNetworkDriverMock
            .Setup(d => d.SendRequestAsync<AiChatResponse>(
                "/v1/chat/completions",
                HttpMethod.Post, 
                Match.Create<AiChatRequest>(r => MatchUtils.MatchBasicObject(r, expectedRequest))))
            .ReturnsAsync(response);

        var actual = await _aiChatService.SendSingleMessageToAiAsync(initialPrompt);

        actual.Should().BeEquivalentTo(expectedDtos);
    }
}
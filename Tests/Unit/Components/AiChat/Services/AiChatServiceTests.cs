using Apotheosis.Components.AiChat.Configuration;
using Apotheosis.Components.AiChat.Interfaces;
using Apotheosis.Components.AiChat.Models;
using Apotheosis.Components.AiChat.Services;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Tests.Utils;

namespace Tests.Unit.Components.AiChat.Services;

public sealed class AiChatServiceTests : IDisposable
{
    private readonly Mock<IAiChatNetworkDriver> _aiChatNetworkDriverMock;
    private readonly Mock<IOptions<AiChatSettings>> _aiChatOptionsMock;
    private readonly IAiChatService _aiChatService;
    
    private readonly AiChatSettings _aiChatSettings = new()
    {
        OpenAiModel = "model",
        ChatSystemInstruction = "example system instruction"
    };

    public AiChatServiceTests()
    {
        _aiChatNetworkDriverMock = new Mock<IAiChatNetworkDriver>(MockBehavior.Strict);
        _aiChatOptionsMock = new Mock<IOptions<AiChatSettings>>(MockBehavior.Strict);
        _aiChatOptionsMock.Setup(o => o.Value).Returns(_aiChatSettings);

        _aiChatService = new AiChatService(_aiChatNetworkDriverMock.Object, _aiChatOptionsMock.Object);
    }

    public void Dispose()
    {
        _aiChatNetworkDriverMock.VerifyAll();
        _aiChatOptionsMock.VerifyAll();
    }

    [Fact]
    public async Task InitializeThreadToAiAsync_CallsNetworkDriverAndReturnsResponseDtos_GivenInitialPrompt()
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

        var actual = await _aiChatService.InitializeThreadToAiAsync(initialPrompt);

        actual.Should().BeEquivalentTo(expectedDtos);
    }
}
using Apotheosis.Components.AiChat.Interfaces;
using Apotheosis.Components.AiChat.Models;
using Apotheosis.Components.Converse.Configuration;
using Apotheosis.Components.Converse.Services;
using Apotheosis.Components.Logging.Interfaces;
using Apotheosis.Components.TextToSpeech.Interfaces;
using FluentAssertions;
using Moq;

namespace Tests.Unit.Components.Converse.Services;

public sealed class ConverseServiceTests : IDisposable
{
    private const string ExampleSystemPrompt = "example system prompt";
    private const string DefaultSystemPrompt = "default system prompt";
    
    private readonly Mock<IAiChatService> _aiChatServiceMock = new(MockBehavior.Strict);
    private readonly Mock<ITextToSpeechService> _textToSpeechServiceMock = new(MockBehavior.Strict);
    private readonly Mock<ILogService<ConverseService>> _loggerMock = new(MockBehavior.Strict);
    
    private readonly ConverseSettings _converseSettings = new()
    {
        VoiceSystemInstructions = new Dictionary<string, string>()
        {
            {"example_voice", ExampleSystemPrompt},
            {"default", DefaultSystemPrompt},
        }
    };

    public void Dispose()
    {
        _aiChatServiceMock.VerifyAll();
        _textToSpeechServiceMock.VerifyAll();
        _loggerMock.VerifyAll();
    }

    [Fact]
    public async Task
        GenerateConverseResponseFromPromptAsync_ReturnsStreamAndUsesCorrectSystemPrompt_GivenValidVoiceInSettings()
    {
        const string prompt = "example prompt";
        const string voiceName = "example_voice";
        const string voiceId = "abc123";
        const string responseMessageContent = "example response";
        var stream = new MemoryStream();
        
        var aiChatServiceResponse = new List<AiChatMessageDto>
        {
            new()
            {
                Index = 0,
                Content = responseMessageContent
            }
        };

        _aiChatServiceMock
            .Setup(acs => acs.SendSingleMessageToAiAsync(prompt, ExampleSystemPrompt))
            .ReturnsAsync(aiChatServiceResponse);

        _textToSpeechServiceMock
            .Setup(tts => tts.GenerateSpeechFromPromptAsync(responseMessageContent, voiceId))
            .ReturnsAsync(stream);
        
        var converseService = new ConverseService(
            _aiChatServiceMock.Object,
            _textToSpeechServiceMock.Object,
            _loggerMock.Object, 
            _converseSettings);

        var actual = await converseService.GenerateConverseResponseFromPromptAsync(prompt, voiceName, voiceId);
        
        actual.Should().BeSameAs(stream);
    }
    
    [Fact]
    public async Task
        GenerateConverseResponseFromPromptAsync_ReturnsStreamAndUsesDefaultSystemPrompt_GivenVoiceNotInSettings()
    {
        const string prompt = "example prompt";
        const string voiceName = "not_real_voice";
        const string voiceId = "abc123";
        const string responseMessageContent = "example response";
        var stream = new MemoryStream();
        
        var aiChatServiceResponse = new List<AiChatMessageDto>
        {
            new()
            {
                Index = 0,
                Content = responseMessageContent
            }
        };

        _aiChatServiceMock
            .Setup(acs => acs.SendSingleMessageToAiAsync(prompt, DefaultSystemPrompt))
            .ReturnsAsync(aiChatServiceResponse);

        _textToSpeechServiceMock
            .Setup(tts => tts.GenerateSpeechFromPromptAsync(responseMessageContent, voiceId))
            .ReturnsAsync(stream);
        
        var converseService = new ConverseService(
            _aiChatServiceMock.Object,
            _textToSpeechServiceMock.Object,
            _loggerMock.Object, 
            _converseSettings);

        var actual = await converseService.GenerateConverseResponseFromPromptAsync(prompt, voiceName, voiceId);
        
        actual.Should().BeSameAs(stream);
    }
    
    [Fact]
    public async Task
        GenerateConverseResponseFromPromptAsync_ReturnsStreamAndUsesNullSystemPrompt_GivenVoiceNotInSettingsAndNoDefault()
    {
        const string prompt = "example prompt";
        const string voiceName = "not_real_voice";
        const string voiceId = "abc123";
        const string responseMessageContent = "example response";
        var stream = new MemoryStream();
        
        var aiChatServiceResponse = new List<AiChatMessageDto>
        {
            new()
            {
                Index = 0,
                Content = responseMessageContent
            }
        };
        
        _loggerMock
            .Setup(l => l.LogWarning(
                null,
                "Default voice system instruction for converse operation not found."))
            .Verifiable();

        _aiChatServiceMock
            .Setup(acs => acs.SendSingleMessageToAiAsync(prompt, null))
            .ReturnsAsync(aiChatServiceResponse);

        _textToSpeechServiceMock
            .Setup(tts => tts.GenerateSpeechFromPromptAsync(responseMessageContent, voiceId))
            .ReturnsAsync(stream);

        var noDefaultConverseSettings = new ConverseSettings
        {
            VoiceSystemInstructions = new Dictionary<string, string>()
        };
        
        var converseService = new ConverseService(
            _aiChatServiceMock.Object,
            _textToSpeechServiceMock.Object,
            _loggerMock.Object, 
            noDefaultConverseSettings);

        var actual = await converseService.GenerateConverseResponseFromPromptAsync(prompt, voiceName, voiceId);
        
        actual.Should().BeSameAs(stream);
    }
}
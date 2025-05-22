using Apotheosis.Core.Features.AiChat.Interfaces;
using Apotheosis.Core.Features.TextToSpeech.Interfaces;

namespace Apotheosis.Test.Unit.Components.Converse.Services;

public sealed class ConverseServiceTests : IDisposable
{
    readonly Mock<IAiChatService> _aiChatServiceMock = new(MockBehavior.Strict);
    readonly Mock<ITextToSpeechService> _textToSpeechServiceMock = new(MockBehavior.Strict);

    public void Dispose()
    {
        _aiChatServiceMock.VerifyAll();
        _textToSpeechServiceMock.VerifyAll();
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
        

        _aiChatServiceMock
            .Setup(acs => acs.GetChatResponseAsync(prompt, CancellationToken.None))
            .ReturnsAsync(responseMessageContent);

        _textToSpeechServiceMock
            .Setup(tts => tts.GenerateSpeechFromPromptAsync(responseMessageContent, voiceId))
            .ReturnsAsync(stream);

        var converseService = new ConverseService(
            _aiChatServiceMock.Object,
            _textToSpeechServiceMock.Object);

        var actual = await converseService.GenerateConverseResponseFromPromptAsync(prompt, voiceName, voiceId, CancellationToken.None);

        actual.Should().BeSameAs(stream);
    }
}
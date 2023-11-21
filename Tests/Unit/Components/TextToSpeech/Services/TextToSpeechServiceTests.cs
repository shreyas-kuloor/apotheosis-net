using Apotheosis.Components.TextToSpeech.Configuration;
using Apotheosis.Components.TextToSpeech.Interfaces;
using Apotheosis.Components.TextToSpeech.Models;
using Apotheosis.Components.TextToSpeech.Services;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;

namespace Tests.Unit.Components.TextToSpeech.Services;

public sealed class TextToSpeechServiceTests : IDisposable
{
    private readonly Mock<IOptions<TextToSpeechSettings>> _textToSpeechOptionsMock;
    private readonly Mock<ITextToSpeechNetworkDriver> _textToSpeechNetworkDriverMock;
    private readonly ITextToSpeechService _textToSpeechService;
    
    private readonly TextToSpeechSettings _textToSpeechSettings = new()
    {
        ElevenLabsModelId = "test-model",
        ElevenLabsStability = 0.1,
        ElevenLabsSimilarityBoost = 0.1,
        ElevenLabsStyle = 0.1
    };

    public TextToSpeechServiceTests()
    {
        _textToSpeechOptionsMock = new Mock<IOptions<TextToSpeechSettings>>(MockBehavior.Strict);
        _textToSpeechOptionsMock.Setup(o => o.Value).Returns(_textToSpeechSettings);
        _textToSpeechNetworkDriverMock = new Mock<ITextToSpeechNetworkDriver>(MockBehavior.Strict);
        _textToSpeechService = new TextToSpeechService(
            _textToSpeechOptionsMock.Object, 
            _textToSpeechNetworkDriverMock.Object);
    }

    public void Dispose()
    {
        _textToSpeechOptionsMock.VerifyAll();
        _textToSpeechNetworkDriverMock.VerifyAll();
    }

    [Fact]
    public async Task GenerateSpeechFromPromptAsync_ReturnsStream_GivenNetworkDriverReturnsStream()
    {
        const string prompt = "test prompt";
        const string voiceId = "voice";
        var stream = new MemoryStream();
        
        _textToSpeechNetworkDriverMock.Setup(d => d.SendRequestReceiveStreamAsync(
            $"/v1/text-to-speech/{voiceId}/stream",
            HttpMethod.Post,
            Match.Create<TextToSpeechRequest>(r =>
                r.Text == prompt
                && r.ModelId == _textToSpeechSettings.ElevenLabsModelId
                && Math.Abs(r.VoiceSettings!.Stability - _textToSpeechSettings.ElevenLabsStability) < 0.001
                && Math.Abs(r.VoiceSettings!.SimilarityBoost - _textToSpeechSettings.ElevenLabsSimilarityBoost) < 0.001
                && Math.Abs(r.VoiceSettings!.Style - _textToSpeechSettings.ElevenLabsStyle) < 0.001)))
            .ReturnsAsync(stream);

        var actual = await _textToSpeechService.GenerateSpeechFromPromptAsync(prompt, voiceId);

        actual.Should().BeSameAs(stream);
    }
    
    [Fact]
    public async Task GetVoicesAsync_ReturnsEnumerableOfVoiceDto_GivenNetworkDriverReturnsResponse()
    {
        var voicesResponse = new VoicesResponse
        {
            Voices = new List<VoiceResponse>
            {
                new()
                {
                    VoiceId = "1",
                    Name = "Voice 1"
                },
                new()
                {
                    VoiceId = "2",
                    Name = "Voice 2"
                }
            }
        };

        var expectedDtoList = new List<VoiceDto>
        {
            new()
            {
                VoiceId = "1",
                VoiceName = "Voice 1"
            },
            new()
            {
                VoiceId = "2",
                VoiceName = "Voice 2"
            }
        };
        
        _textToSpeechNetworkDriverMock.Setup(d => d.SendRequestAsync<VoicesResponse>(
                "/v1/voices", 
                HttpMethod.Get, 
                null))
            .ReturnsAsync(voicesResponse);

        var actual = await _textToSpeechService.GetVoicesAsync();

        actual.Should().BeEquivalentTo(expectedDtoList);
    }
}
using Apotheosis.Core.Features.TextToSpeech.Interfaces;
using Apotheosis.Core.Features.TextToSpeech.Models;
using Apotheosis.Server.Features.TextToSpeech.Configuration;

namespace Apotheosis.Server.Features.TextToSpeech.Services;

[Scoped]
public sealed class TextToSpeechService(
    IOptions<TextToSpeechSettings> textToSpeechOptions,
    ITextToSpeechNetworkDriver textToSpeechNetworkDriver)
    : ITextToSpeechService
{
    readonly TextToSpeechSettings _textToSpeechSettings = textToSpeechOptions.Value;

    public async Task<Stream> GenerateSpeechFromPromptAsync(string prompt, string voiceId)
    {
        var ttsRequest = new TextToSpeechRequest
        {
            Text = prompt,
            ModelId = _textToSpeechSettings.ElevenLabsModelId,
            VoiceSettings = new TextToSpeechVoiceSettingsRequest
            {
                Stability = _textToSpeechSettings.ElevenLabsStability,
                SimilarityBoost = _textToSpeechSettings.ElevenLabsSimilarityBoost,
                Style = _textToSpeechSettings.ElevenLabsStyle,
            }
        };

        var voiceStream =
            await textToSpeechNetworkDriver.SendRequestReceiveStreamAsync(
                $"/v1/text-to-speech/{voiceId}/stream",
                HttpMethod.Post,
                ttsRequest);

        return voiceStream;
    }

    public async Task<IEnumerable<VoiceDto>> GetVoicesAsync()
    {
        var voicesResponse = await textToSpeechNetworkDriver.SendRequestAsync<VoicesResponse>(
            "/v1/voices",
            HttpMethod.Get,
            null);

        var voices = voicesResponse.Voices!.Select(v => new VoiceDto { VoiceId = v.VoiceId, VoiceName = v.Name });

        return voices;
    }
}
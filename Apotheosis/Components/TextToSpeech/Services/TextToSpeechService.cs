using Apotheosis.Components.TextToSpeech.Configuration;
using Apotheosis.Components.TextToSpeech.Interfaces;
using Apotheosis.Components.TextToSpeech.Models;

namespace Apotheosis.Components.TextToSpeech.Services;

public sealed class TextToSpeechService(
    TextToSpeechSettings textToSpeechSettings,
    ITextToSpeechNetworkDriver textToSpeechNetworkDriver)
    : ITextToSpeechService
{
    public async Task<Stream> GenerateSpeechFromPromptAsync(string prompt, string voiceId)
    {
        var ttsRequest = new TextToSpeechRequest
        {
            Text = prompt,
            ModelId = textToSpeechSettings.ElevenLabsModelId,
            VoiceSettings = new TextToSpeechVoiceSettingsRequest
            {
                Stability = textToSpeechSettings.ElevenLabsStability,
                SimilarityBoost = textToSpeechSettings.ElevenLabsSimilarityBoost,
                Style = textToSpeechSettings.ElevenLabsStyle,
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
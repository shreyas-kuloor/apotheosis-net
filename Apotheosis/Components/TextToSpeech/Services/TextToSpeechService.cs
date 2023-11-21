﻿using Apotheosis.Components.TextToSpeech.Configuration;
using Apotheosis.Components.TextToSpeech.Interfaces;
using Apotheosis.Components.TextToSpeech.Models;
using Microsoft.Extensions.Options;

namespace Apotheosis.Components.TextToSpeech.Services;

public sealed class TextToSpeechService : ITextToSpeechService
{
    private readonly TextToSpeechSettings _textToSpeechSettings;
    private readonly ITextToSpeechNetworkDriver _textToSpeechNetworkDriver;

    public TextToSpeechService(
        IOptions<TextToSpeechSettings> textToSpeechOptions, 
        ITextToSpeechNetworkDriver textToSpeechNetworkDriver)
    {
        _textToSpeechSettings = textToSpeechOptions.Value;
        _textToSpeechNetworkDriver = textToSpeechNetworkDriver;
    }
    
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
            await _textToSpeechNetworkDriver.SendRequestReceiveStreamAsync(
                $"/v1/text-to-speech/{voiceId}/stream", 
                HttpMethod.Post,
                ttsRequest);

        return voiceStream;
    }

    public async Task<IEnumerable<VoiceDto>> GetVoicesAsync()
    {
        var voicesResponse = await _textToSpeechNetworkDriver.SendRequestAsync<VoicesResponse>(
            "/v1/voices", 
            HttpMethod.Get, 
            null);

        var voices = voicesResponse.Voices!.Select(v => new VoiceDto { VoiceId = v.VoiceId, VoiceName = v.Name });

        return voices;
    }
}
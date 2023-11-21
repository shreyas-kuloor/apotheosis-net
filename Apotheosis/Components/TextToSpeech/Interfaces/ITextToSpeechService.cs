using Apotheosis.Components.TextToSpeech.Models;

namespace Apotheosis.Components.TextToSpeech.Interfaces;

public interface ITextToSpeechService
{
    Task<Stream> GenerateSpeechFromPromptAsync(string prompt, string voiceId);

    Task<IEnumerable<VoiceDto>> GetVoicesAsync();
}
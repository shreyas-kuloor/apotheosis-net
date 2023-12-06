using Apotheosis.Core.Components.TextToSpeech.Models;

namespace Apotheosis.Core.Components.TextToSpeech.Interfaces;

public interface ITextToSpeechService
{
    Task<Stream> GenerateSpeechFromPromptAsync(string prompt, string voiceId);

    Task<IEnumerable<VoiceDto>> GetVoicesAsync();
}
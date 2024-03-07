using Apotheosis.Core.Features.TextToSpeech.Models;

namespace Apotheosis.Core.Features.TextToSpeech.Interfaces;

public interface ITextToSpeechService
{
    Task<Stream> GenerateSpeechFromPromptAsync(string prompt, string voiceId);

    Task<IEnumerable<VoiceDto>> GetVoicesAsync();
}
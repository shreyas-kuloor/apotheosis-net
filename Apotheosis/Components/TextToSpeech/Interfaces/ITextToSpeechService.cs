namespace Apotheosis.Components.TextToSpeech.Interfaces;

public interface ITextToSpeechService
{
    Task<Stream> GenerateSpeechFromPromptAsync(string prompt, string voiceId);
}
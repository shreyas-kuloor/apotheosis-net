namespace Apotheosis.Core.Components.Converse.Interfaces;

public interface IConverseService
{
    Task<Stream> GenerateConverseResponseFromPromptAsync(string prompt, string voiceName, string voiceId);
}
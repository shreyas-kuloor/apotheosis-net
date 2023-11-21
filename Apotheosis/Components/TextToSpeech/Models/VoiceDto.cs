namespace Apotheosis.Components.TextToSpeech.Models;

public sealed class VoiceDto
{
    /// <summary>
    /// Gets or sets the ID of the voice.
    /// </summary>
    /// <returns></returns>
    public string? VoiceId { get; set; }
    
    /// <summary>
    /// Gets or sets the name of the voice.
    /// </summary>
    public string? VoiceName { get; set; }
}
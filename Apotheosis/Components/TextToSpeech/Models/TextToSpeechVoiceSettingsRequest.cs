namespace Apotheosis.Components.TextToSpeech.Models;

public sealed class TextToSpeechVoiceSettingsRequest
{
    /// <summary>
    /// Gets or sets the stability value.
    /// </summary>
    public double Stability { get; set; }
    
    /// <summary>
    /// Gets or sets the similarity boost value.
    /// </summary>
    public double SimilarityBoost { get; set; }
    
    /// <summary>
    /// Gets or sets the style value.
    /// </summary>
    public double Style { get; set; }
}
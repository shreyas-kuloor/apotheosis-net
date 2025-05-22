namespace Apotheosis.Core.Features.TextToSpeech.Models;

public sealed class TextToSpeechVoiceSettingsRequest
{
    /// <summary>
    /// Gets or sets the stability value.
    /// </summary>
    [JsonProperty("stability")]
    public double Stability { get; set; }

    /// <summary>
    /// Gets or sets the similarity boost value.
    /// </summary>
    [JsonProperty("similarity_boost")]
    public double SimilarityBoost { get; set; }

    /// <summary>
    /// Gets or sets the style value.
    /// </summary>
    [JsonProperty("style")]
    public double Style { get; set; }
}
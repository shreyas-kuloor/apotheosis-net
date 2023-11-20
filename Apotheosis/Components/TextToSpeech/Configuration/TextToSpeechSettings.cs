namespace Apotheosis.Components.TextToSpeech.Configuration;

public sealed class TextToSpeechSettings
{
    /// <summary>
    /// The base URL for Eleven Labs requests.
    /// </summary>
    public Uri? ElevenLabsBaseUrl { get; set; }
    
    /// <summary>
    /// The API Key for Eleven Labs requests.
    /// </summary>
    public string? ElevenLabsApiKey { get; set; }
    
    /// <summary>
    /// The model ID for Eleven Labs requests.
    /// </summary>
    public string? ElevenLabsModelId { get; set; }
    
    /// <summary>
    /// The stability value for Eleven Labs requests.
    /// </summary>
    public double ElevenLabsStability { get; set; }
    
    /// <summary>
    /// The similarity boost value for Eleven Labs requests.
    /// </summary>
    public double ElevenLabsSimilarityBoost { get; set; }
    
    /// <summary>
    /// The style value for Eleven Labs requests.
    /// </summary>
    public double ElevenLabsStyle { get; set; }
    
    /// <summary>
    /// Gets or sets the output format for audio stream.
    /// </summary>
    public string? OutputFormat { get; set; }
}
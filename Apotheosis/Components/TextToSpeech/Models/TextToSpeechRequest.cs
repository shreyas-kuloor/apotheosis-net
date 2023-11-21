using Newtonsoft.Json;

namespace Apotheosis.Components.TextToSpeech.Models;

public sealed class TextToSpeechRequest
{
    /// <summary>
    /// Gets or sets the text of the request.
    /// </summary>
    [JsonProperty("text")]
    public string? Text { get; set; }
    
    /// <summary>
    /// Gets or sets the model ID value for the request.
    /// </summary>
    [JsonProperty("model_id")]
    public string? ModelId { get; set; }
    
    /// <summary>
    /// Gets or sets the voice settings for the request.
    /// </summary>
    [JsonProperty("voice_settings")]
    public TextToSpeechVoiceSettingsRequest? VoiceSettings { get; set; }
}
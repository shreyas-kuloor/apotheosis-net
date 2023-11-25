using Newtonsoft.Json;

namespace Apotheosis.Components.TextToSpeech.Models;

public sealed class VoiceResponse
{
    /// <summary>
    /// Gets or sets the voice ID of the response.
    /// </summary>
    /// <returns></returns>
    [JsonProperty("voice_id")]
    public string? VoiceId { get; set; }
    
    /// <summary>
    /// Gets or sets the voice name of the response.
    /// </summary>
    public string? Name { get; set; }
}
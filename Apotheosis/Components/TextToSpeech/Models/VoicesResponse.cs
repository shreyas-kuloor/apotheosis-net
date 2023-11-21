namespace Apotheosis.Components.TextToSpeech.Models;

public sealed class VoicesResponse
{
    /// <summary>
    /// Gets or sets the list of voices of the response.
    /// </summary>
    public IEnumerable<VoiceResponse>? Voices { get; set; }
}
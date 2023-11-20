using Apotheosis.Components.Client.Configuration;
using Apotheosis.Components.GCPDot.Configuration;
using Apotheosis.Components.TextToSpeech.Configuration;

namespace Apotheosis.Configuration;

public sealed class AppSettings
{
    /// <summary>
    /// Gets or sets client related settings.
    /// </summary>
    public ClientSettings? Client { get; set; }
    
    /// <summary>
    /// Gets or sets GCP dot related settings.
    /// </summary>
    public GcpDotSettings? GcpDot{ get; set; }
    
    /// <summary>
    /// Gets or sets text to speech related settings.
    /// </summary>
    public TextToSpeechSettings? TextToSpeech { get; set; }
}

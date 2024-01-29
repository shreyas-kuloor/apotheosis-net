using Apotheosis.Core.Components.AiChat.Configuration;
using Apotheosis.Core.Components.Converse.Configuration;
using Apotheosis.Core.Components.GCPDot.Configuration;
using Apotheosis.Core.Components.ImageGen.Configuration;
using Apotheosis.Core.Components.TextToSpeech.Configuration;

namespace Apotheosis.Core.Configuration;

public sealed class AppSettings
{   
    /// <summary>
    /// Gets or sets GCP dot related settings.
    /// </summary>
    public GcpDotSettings? GcpDot{ get; set; }
    
    /// <summary>
    /// Gets or sets text to speech related settings.
    /// </summary>
    public TextToSpeechSettings? TextToSpeech { get; set; }
    
    /// <summary>
    /// Gets or sets image generation related settings.
    /// </summary>
    public ImageGenSettings? ImageGen { get; set; }
    
    /// <summary>
    /// Gets or sets AI chat related settings.
    /// </summary>
    public AiChatSettings? AiChat { get; set; }
    
    /// <summary>
    /// Gets or sets converse related settings.
    /// </summary>
    public ConverseSettings? Converse { get; set; }
}

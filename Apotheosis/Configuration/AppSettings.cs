using Apotheosis.Components.AiChat.Configuration;
using Apotheosis.Components.Client.Configuration;

namespace Apotheosis.Configuration;

public class AppSettings
{
    /// <summary>
    /// Gets or sets client related settings.
    /// </summary>
    public ClientSettings Client { get; set; }
    
    /// <summary>
    /// Gets or sets AI chat related settings.
    /// </summary>
    public AiChatSettings AiChat { get; set; }
}

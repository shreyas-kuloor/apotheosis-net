namespace Apotheosis.Server.Features.MediaRequest.Configuration;

public sealed class RadarrSettings
{
    public const string Name = "Radarr";

    /// <summary>
    /// Gets or sets the Radarr Base URL.
    /// </summary>
    public Uri? BaseUrl { get; set; }

    /// <summary>
    /// Gets or sets the Radarr API key.
    /// </summary>
    public string? ApiKey { get; set; }
}
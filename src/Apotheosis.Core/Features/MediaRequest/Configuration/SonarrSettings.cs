namespace Apotheosis.Core.Features.MediaRequest.Configuration;

public sealed class SonarrSettings
{
    public const string Name = "Sonarr";

    /// <summary>
    /// Gets or sets the Sonarr Base URL.
    /// </summary>
    public Uri? BaseUrl { get; set; }

    /// <summary>
    /// Gets or sets the Sonarr API key.
    /// </summary>
    public string? ApiKey { get; set; }
}
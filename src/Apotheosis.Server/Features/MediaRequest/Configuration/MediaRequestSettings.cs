namespace Apotheosis.Server.Features.MediaRequest.Configuration;

public sealed class MediaRequestSettings
{
    public const string Name = "MediaRequest";

    /// <summary>
    /// Gets or sets the media request cache expiration in minutes.
    /// </summary>
    public int CacheExpirationMinutes { get; set; }

    /// <summary>
    /// Gets or sets the number of options for a media request.
    /// </summary>
    public int OptionCount { get; set; }

    /// <summary>
    /// Gets or sets the quality profile name to use for a media request.
    /// </summary>
    public string? QualityProfileName { get; set; }

}
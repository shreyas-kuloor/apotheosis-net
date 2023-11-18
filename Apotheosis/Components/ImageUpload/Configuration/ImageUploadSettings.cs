namespace Apotheosis.Components.ImageUpload.Configuration;

public sealed class ImageUploadSettings
{
    /// <summary>
    /// The base URL for image upload requests.
    /// </summary>
    public Uri? BaseUrl { get; set; }
    
    /// <summary>
    /// The client ID for image upload requests.
    /// </summary>
    public string? ClientId { get; set; }
    
    /// <summary>
    /// The client secret for image upload requests.
    /// </summary>
    public string? ClientSecret { get; set; }
    
    /// <summary>
    /// The refresh token for image upload requests.
    /// </summary>
    public string? RefreshToken { get; set; }
}
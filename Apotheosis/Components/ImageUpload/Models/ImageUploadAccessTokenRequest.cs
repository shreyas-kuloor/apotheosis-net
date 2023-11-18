using Newtonsoft.Json;

namespace Apotheosis.Components.ImageUpload.Models;

public sealed class ImageUploadAccessTokenRequest
{
    /// <summary>
    /// Gets or sets the refresh token.
    /// </summary>
    [JsonProperty("refresh_token")]
    public string? RefreshToken { get; set; }
    
    /// <summary>
    /// Gets or sets the grant type.
    /// </summary>
    [JsonProperty("grant_type")]
    public string? GrantType { get; set; }
    
    /// <summary>
    /// Gets or sets the client ID.
    /// </summary>
    [JsonProperty("client_id")]
    public string? ClientId { get; set; }
    
    /// <summary>
    /// Gets or sets the client secret.
    /// </summary>
    [JsonProperty("client_secret")]
    public string? ClientSecret { get; set; }
}
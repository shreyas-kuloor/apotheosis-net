using Newtonsoft.Json;

namespace Apotheosis.Components.ImageUpload.Models;

public sealed class ImageUploadAccessTokenResponse
{
    /// <summary>
    /// Gets or sets the access token.
    /// </summary>
    [JsonProperty("access_token")]
    public string? AccessToken { get; set; }
    
    /// <summary>
    /// Gets or sets the client ID.
    /// </summary>
    [JsonProperty("expires_in")]
    public long ExpiresIn { get; set; }
    
    /// <summary>
    /// Gets or sets the token type.
    /// </summary>
    [JsonProperty("token_type")]
    public string? TokenType { get; set; }
    
    /// <summary>
    /// Gets or sets the scope.
    /// </summary>
    [JsonProperty("scope")]
    public string? Scope { get; set; }
    
    /// <summary>
    /// Gets or sets the refresh token.
    /// </summary>
    [JsonProperty("refresh_token")]
    public string? RefreshToken { get; set; }
    
    /// <summary>
    /// Gets or sets the account ID.
    /// </summary>
    [JsonProperty("account_id")]
    public long AccountId { get; set; }
    
    /// <summary>
    /// Gets or sets the account username.
    /// </summary>
    [JsonProperty("account_username")]
    public string? AccountUsername { get; set; }
}
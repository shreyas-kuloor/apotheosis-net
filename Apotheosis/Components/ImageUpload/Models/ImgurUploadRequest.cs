using Newtonsoft.Json;

namespace Apotheosis.Components.ImageUpload.Models;

public sealed class ImgurUploadRequest
{
    /// <summary>
    /// Gets or sets the image to upload to Imgur.
    /// </summary>
    [JsonProperty("image")]
    public string? Image { get; set; }
}
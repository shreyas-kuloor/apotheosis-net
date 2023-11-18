using Apotheosis.Components.ImageUpload.Interfaces;
using Apotheosis.Components.ImageUpload.Models;

namespace Apotheosis.Components.ImageUpload.Services;

public sealed class ImageUploadService : IImageUploadService
{
    private readonly IImageUploadNetworkDriver _imageUploadNetworkDriver;
    
    public ImageUploadService(IImageUploadNetworkDriver imageUploadNetworkDriver)
    {
        _imageUploadNetworkDriver = imageUploadNetworkDriver;
    }
    
    public async Task<Uri> UploadImageAsync(string base64Image)
    {
        var uploadImageRequest = new ImgurUploadRequest
        {
            Image = base64Image
        };

        var uploadImageResponse =
            await _imageUploadNetworkDriver.SendRequestAsync<ImgurUploadRequest, ImgurUploadResponse>("/3/image", HttpMethod.Post, uploadImageRequest);

        return uploadImageResponse.Data!.Link!;
    }
}
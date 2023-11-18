namespace Apotheosis.Components.ImageUpload.Interfaces;

public interface IImageUploadService
{
    Task<Uri> UploadImageAsync(string base64Image);
}
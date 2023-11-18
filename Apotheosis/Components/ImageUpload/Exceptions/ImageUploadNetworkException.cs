namespace Apotheosis.Components.ImageUpload.Exceptions;

public class ImageUploadNetworkException : Exception
{
    public ImageUploadNetworkException()
    {
    }

    public ImageUploadNetworkException(string message) : base(message)
    {
    }

    public ImageUploadNetworkException(string? message, Exception? inner)
        : base(message, inner)
    {
    }
}
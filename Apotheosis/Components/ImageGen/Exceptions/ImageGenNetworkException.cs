namespace Apotheosis.Components.ImageGen.Exceptions;

public class ImageGenNetworkException : Exception
{
    public ImageGenNetworkException()
    {
    }

    public ImageGenNetworkException(string message) : base(message)
    {
    }

    public ImageGenNetworkException(string? message, Exception? inner)
        : base(message, inner)
    {
    }
}
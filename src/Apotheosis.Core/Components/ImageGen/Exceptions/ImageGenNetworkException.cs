namespace Apotheosis.Core.Components.ImageGen.Exceptions;

public sealed class ImageGenNetworkException : Exception
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
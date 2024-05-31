using Apotheosis.Core.Features.ImageGen.Exceptions;
using FluentAssertions;

namespace Apotheosis.Test.Unit.Components.ImageGen.Exceptions;

public sealed class ImageGenNetworkExceptionTests
{
    [Fact]
    public void ConstructorWithMessage_CreatesNewImageGenNetworkException()
    {
        const string message = "Image Gen Network Exception";
        var exception = new ImageGenNetworkException(message);

        exception.Message.Should().BeEquivalentTo(message);
        exception.InnerException.Should().BeNull();
    }

    [Fact]
    public void ConstructorWithMessageAndInnerException_CreatesNewImageGenNetworkException()
    {
        const string message = "Image Gen Network Exception";
        var innerException = new Exception();
        var exception = new ImageGenNetworkException(message, innerException);

        exception.Message.Should().BeEquivalentTo(message);
        exception.InnerException.Should().BeEquivalentTo(innerException);
    }
}
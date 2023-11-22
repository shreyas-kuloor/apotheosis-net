using Apotheosis.Components.ImageGen.Exceptions;
using FluentAssertions;

namespace Tests.Unit.Components.ImageGen.Exceptions;

public class ImageGenNetworkExceptionTests
{
    [Fact]
    public void ConstructorWithMessage_CreatesNewGcpDotException()
    {
        const string message = "Image Gen Network Exception";
        var exception = new ImageGenNetworkException(message);

        exception.Message.Should().BeEquivalentTo(message);
        exception.InnerException.Should().BeNull();
    }
    
    [Fact]
    public void ConstructorWithMessageAndInnerException_CreatesNewGcpDotException()
    {
        const string message = "Image Gen Network Exception";
        var innerException = new Exception();
        var exception = new ImageGenNetworkException(message, innerException);
        
        exception.Message.Should().BeEquivalentTo(message);
        exception.InnerException.Should().BeEquivalentTo(innerException);
    }
}
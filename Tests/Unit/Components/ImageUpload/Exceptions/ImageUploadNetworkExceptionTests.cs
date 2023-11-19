using Apotheosis.Components.ImageUpload.Exceptions;
using FluentAssertions;

namespace Tests.Unit.Components.ImageUpload.Exceptions;

public class ImageUploadNetworkExceptionTests
{
    [Fact]
    public void ConstructorWithMessage_CreatesNewImageUploadException()
    {
        const string message = "Image Upload Network Exception";
        var exception = new ImageUploadNetworkException(message);

        exception.Message.Should().BeEquivalentTo(message);
        exception.InnerException.Should().BeNull();
    }
    
    [Fact]
    public void ConstructorWithMessageAndInnerException_CreatesNewImageUploadException()
    {
        const string message = "Image Upload Network Exception";
        var innerException = new Exception();
        var exception = new ImageUploadNetworkException(message, innerException);
        
        exception.Message.Should().BeEquivalentTo(message);
        exception.InnerException.Should().BeEquivalentTo(innerException);
    }
}
using Apotheosis.Core.Features.AiChat.Exceptions;
using FluentAssertions;

namespace Apotheosis.Test.Unit.Features.AiChat.Exceptions;

public sealed class AiThreadChannelStoreExceptionTests
{
    [Fact]
    public void ConstructorWithMessage_CreatesNewAiThreadChannelStoreException()
    {
        const string message = "AI Thread Channel Store Exception";
        var exception = new AiThreadChannelStoreException(message);

        exception.Message.Should().BeEquivalentTo(message);
        exception.InnerException.Should().BeNull();
    }
    
    [Fact]
    public void ConstructorWithMessageAndInnerException_CreatesNewAiThreadChannelStoreException()
    {
        const string message = "AI Thread Channel Store Exception";
        var innerException = new Exception();
        var exception = new AiThreadChannelStoreException(message, innerException);
        
        exception.Message.Should().BeEquivalentTo(message);
        exception.InnerException.Should().BeEquivalentTo(innerException);
    }
}
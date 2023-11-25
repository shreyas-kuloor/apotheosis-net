using Apotheosis.Components.AiChat.Exceptions;
using FluentAssertions;

namespace Tests.Unit.Components.AiChat.Exceptions;

public class AiChatNetworkExceptionTests
{
    [Fact]
    public void ConstructorWithMessage_CreatesNewAiChatNetworkException()
    {
        const string message = "AI Chat Network Exception";
        var exception = new AiChatNetworkException(message);

        exception.Message.Should().BeEquivalentTo(message);
        exception.InnerException.Should().BeNull();
    }
    
    [Fact]
    public void ConstructorWithMessageAndInnerException_CreatesNewAiChatNetworkException()
    {
        const string message = "AI Chat Network Exception";
        var innerException = new Exception();
        var exception = new AiChatNetworkException(message, innerException);
        
        exception.Message.Should().BeEquivalentTo(message);
        exception.InnerException.Should().BeEquivalentTo(innerException);
    }
    
    [Fact]
    public void ConstructorWithMessageAndReasonAndInnerException_CreatesNewAiChatNetworkException()
    {
        const string message = "AI Chat Network Exception";
        const AiChatNetworkException.ErrorReason reason = AiChatNetworkException.ErrorReason.Unknown;
        var innerException = new Exception();
        var exception = new AiChatNetworkException(message, reason, innerException);
        
        exception.Message.Should().BeEquivalentTo(message);
        exception.InnerException.Should().BeEquivalentTo(innerException);
        exception.Reason.Should().Be(AiChatNetworkException.ErrorReason.Unknown);
    }
}
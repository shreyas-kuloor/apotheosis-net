using Apotheosis.Components.TextToSpeech.Exceptions;
using FluentAssertions;

namespace Tests.Unit.Components.TextToSpeech.Exceptions;

public class TextToSpeechNetworkExceptionTests
{
    [Fact]
    public void ConstructorWithMessage_CreatesNewTextToSpeechNetworkException()
    {
        const string message = "Text to speech Network Exception";
        var exception = new TextToSpeechNetworkException(message);

        exception.Message.Should().BeEquivalentTo(message);
        exception.InnerException.Should().BeNull();
    }
    
    [Fact]
    public void ConstructorWithMessageAndInnerException_CreatesNewTextToSpeechNetworkException()
    {
        const string message = "Text to speech Network Exception";
        var innerException = new Exception();
        var exception = new TextToSpeechNetworkException(message, innerException);
        
        exception.Message.Should().BeEquivalentTo(message);
        exception.InnerException.Should().BeEquivalentTo(innerException);
    }
}
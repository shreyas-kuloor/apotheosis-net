using Apotheosis.Core.Features.Audio.Exceptions;
using FluentAssertions;

namespace Apotheosis.Test.Unit.Features.Audio.Exceptions;

public sealed class VoiceChannelStoreExceptionTests
{
    [Fact]
    public void ConstructorWithMessage_CreatesNewVoiceChannelStoreException()
    {
        const string message = "Voice Channel Store Exception";
        var exception = new VoiceChannelStoreException(message);

        exception.Message.Should().BeEquivalentTo(message);
        exception.InnerException.Should().BeNull();
    }
    
    [Fact]
    public void ConstructorWithMessageAndInnerException_CreatesNewVoiceChannelStoreException()
    {
        const string message = "Voice Channel Store Exception";
        var innerException = new Exception();
        var exception = new VoiceChannelStoreException(message, innerException);
        
        exception.Message.Should().BeEquivalentTo(message);
        exception.InnerException.Should().BeEquivalentTo(innerException);
    }
}
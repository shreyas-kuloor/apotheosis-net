using Apotheosis.Components.GCPDot.Exceptions;
using FluentAssertions;

namespace Tests.Unit.Components.GcpDot.Exceptions;

public class GcpDotNetworkExceptionTests
{
    [Fact]
    public void ConstructorWithMessage_CreatesNewGcpDotException()
    {
        const string message = "Gcp Dot Network Exception";
        var exception = new GcpDotNetworkException(message);

        exception.Message.Should().BeEquivalentTo(message);
        exception.InnerException.Should().BeNull();
    }
    
    [Fact]
    public void ConstructorWithMessageAndInnerException_CreatesNewGcpDotException()
    {
        const string message = "Gcp Dot Network Exception";
        var innerException = new Exception();
        var exception = new GcpDotNetworkException(message, innerException);
        
        exception.Message.Should().BeEquivalentTo(message);
        exception.InnerException.Should().BeEquivalentTo(innerException);
    }
}
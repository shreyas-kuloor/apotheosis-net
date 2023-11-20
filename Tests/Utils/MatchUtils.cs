using FluentAssertions;
using FluentAssertions.Execution;

namespace Tests.Utils;

public static class MatchUtils
{
    public static bool MatchRequestContent(HttpContent actualContent, HttpContent expectedContent)
    {
        try
        {
            actualContent.Should().BeEquivalentTo(expectedContent);
            return true;
        }
        catch (AssertionFailedException)
        {
            return false;
        }
    }
}
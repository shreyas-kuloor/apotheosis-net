using FluentAssertions;
using FluentAssertions.Execution;

namespace Tests.Utils;

public static class MatchUtils
{
    public static bool MatchBasicObject(object? actual, object? expected)
    {
        try
        {
            actual.Should().BeEquivalentTo(expected);
            return true;
        }
        catch (AssertionFailedException)
        {
            return false;
        }
    }
}
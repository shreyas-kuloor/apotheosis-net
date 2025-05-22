namespace Apotheosis.Test.Unit.Components.GcpDot.Services;

public sealed class GcpStatParserTests
{
    [Fact]
    public void ParseGcpStats_ParsesStringHtmlToGcpStats()
    {
        const string gcpStatsHtmlString =
            "<gcpstats><serverTime>1700007143</serverTime><ss><s t='1700007060'>0.001</s><s t='1700007061'>0.8213845</s></ss></gcpstats>";

        var expectedGcpStats = new List<GcpStat>
        {
            new()
            {
                Time = 1700007060,
                Value = 0.001
            },
            new()
            {
                Time = 1700007061,
                Value = 0.8213845
            }
        };

        var actual = GcpStatParser.ParseGcpStats(gcpStatsHtmlString);

        actual.Should().BeEquivalentTo(expectedGcpStats);
    }
}
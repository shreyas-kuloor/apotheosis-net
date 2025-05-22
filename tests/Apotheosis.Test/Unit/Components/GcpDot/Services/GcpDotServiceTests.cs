using Apotheosis.Core.Features.GcpDot.Interfaces;

namespace Apotheosis.Test.Unit.Components.GcpDot.Services;

public sealed class GcpDotServiceTests : IDisposable
{
    readonly Mock<IGcpDotNetworkDriver> _gcpDotNetworkDriverMock;
    readonly GcpDotService _gcpDotService;

    public static readonly TheoryData<string, string, string> TestData = new()
    {
        {
            "<gcpstats><serverTime>1700007143</serverTime><ss><s t='1700007060'>0.001</s></ss></gcpstats>",
            "#FFA8C0",
            "#FF0064"
        },
        {
            "<gcpstats><serverTime>1700007143</serverTime><ss><s t='1700007060'>0.011</s></ss></gcpstats>",
            "#FF1E1E",
            "#840607"
        },
        {
            "<gcpstats><serverTime>1700007143</serverTime><ss><s t='1700007060'>0.06</s></ss></gcpstats>",
            "#FFB82E",
            "#C95E00"
        },
        {
            "<gcpstats><serverTime>1700007143</serverTime><ss><s t='1700007060'>0.1</s></ss></gcpstats>",
            "#FFD517",
            "#C69000"
        },
        {
            "<gcpstats><serverTime>1700007143</serverTime><ss><s t='1700007060'>0.2</s></ss></gcpstats>",
            "#FFFA40",
            "#C6C300"
        },
        {
            "<gcpstats><serverTime>1700007143</serverTime><ss><s t='1700007060'>0.25</s></ss></gcpstats>",
            "#F9FA00",
            "#B0CC00"
        },
        {
            "<gcpstats><serverTime>1700007143</serverTime><ss><s t='1700007060'>0.35</s></ss></gcpstats>",
            "#AEFA00",
            "#88C200"
        },
        {
            "<gcpstats><serverTime>1700007143</serverTime><ss><s t='1700007060'>0.5</s></ss></gcpstats>",
            "#64FA64",
            "#00A700"
        },
        {
            "<gcpstats><serverTime>1700007143</serverTime><ss><s t='1700007060'>0.91</s></ss></gcpstats>",
            "#64FAAB",
            "#00B5C9"
        },
        {
            "<gcpstats><serverTime>1700007143</serverTime><ss><s t='1700007060'>0.92</s></ss></gcpstats>",
            "#ACF2FF",
            "#21BCF1"
        },
        {
            "<gcpstats><serverTime>1700007143</serverTime><ss><s t='1700007060'>0.95</s></ss></gcpstats>",
            "#0EEEFF",
            "#0786E1"
        },
        {
            "<gcpstats><serverTime>1700007143</serverTime><ss><s t='1700007060'>0.97</s></ss></gcpstats>",
            "#24CBFD",
            "#0000FF"
        },
        {
            "<gcpstats><serverTime>1700007143</serverTime><ss><s t='1700007060'>0.99</s></ss></gcpstats>",
            "#5655CA",
            "#2400A0"
        },
        {
            "<gcpstats><serverTime>1700007143</serverTime><ss><s t='1700007060'>0</s></ss></gcpstats>",
            "#CDCDCD",
            "#505050"
        }
    };

    public GcpDotServiceTests()
    {
        _gcpDotNetworkDriverMock = new Mock<IGcpDotNetworkDriver>(MockBehavior.Strict);
        _gcpDotService = new GcpDotService(_gcpDotNetworkDriverMock.Object);
    }

    public void Dispose()
    {
        _gcpDotNetworkDriverMock.VerifyAll();
    }

    [Theory]
    [MemberData(nameof(TestData))]
    public async Task GetGcpDotAsync_ReturnsCorrectCenterAndEdgeColor_GivenGcpStatFromNetwork(
        string gcpStatsResponse,
        string expectedCenterColorHex,
        string expectedEdgeColorHex)
    {
        var expectedCenterColor = Color.Parse(expectedCenterColorHex);
        var expectedEdgeColor = Color.Parse(expectedEdgeColorHex);

        _gcpDotNetworkDriverMock.Setup(networkDriver =>
                networkDriver.SendRequestAsync(GcpDotService.GcpPath, HttpMethod.Get, null))
            .ReturnsAsync(gcpStatsResponse);

        var (actualCenterColor, actualEdgeColor) = await _gcpDotService.GetGcpDotAsync();

        actualCenterColor.Should().BeEquivalentTo(expectedCenterColor);
        actualEdgeColor.Should().BeEquivalentTo(expectedEdgeColor);
    }
}
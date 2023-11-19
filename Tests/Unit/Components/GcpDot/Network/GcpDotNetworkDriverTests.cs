using System.Net;
using Apotheosis.Components.GCPDot.Configuration;
using Apotheosis.Components.GCPDot.Network;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;

namespace Tests.Unit.Components.GcpDot.Network;

public sealed class GcpDotNetworkDriverTests
{
    private const string RequestString = "request123";
    private const string ResponseString = "response123";
    
    
    private readonly Mock<IOptions<GcpDotSettings>> _gcpDotOptionsMock;
    private readonly Mock<HttpMessageHandler> _mockHandler;
    private readonly HttpClient _httpClient;
    
    private readonly GcpDotSettings _gcpDotSettings = new()
    {
        BaseUrl = new Uri("https://example.com/"),
    };

    public GcpDotNetworkDriverTests()
    {
        _gcpDotOptionsMock = new Mock<IOptions<GcpDotSettings>>(MockBehavior.Strict);
        _mockHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        _httpClient = new HttpClient(_mockHandler.Object);
    }

    [Fact]
    public async Task SendRequestAsync_SendsRequestAndReturnsResponseData_GivenSuccessfulHttpRequestWithNullBody()
    {
        var httpMethod = HttpMethod.Get;
        
        var mockResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(ResponseString)
        };

        _mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(m => m.Method == httpMethod),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(mockResponse);


        _gcpDotOptionsMock.Setup(o => o.Value).Returns(_gcpDotSettings);
        
        var gcpDotNetworkDriver = new GcpDotNetworkDriver(_httpClient, _gcpDotOptionsMock.Object);

        var actual = await gcpDotNetworkDriver.SendRequestAsync("/path", httpMethod, null);
        
        actual.Should().BeEquivalentTo(ResponseString);
        
        _mockHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(m => m.Method == httpMethod),
            ItExpr.IsAny<CancellationToken>());
    }
    
    [Fact]
    public async Task SendRequestAsync_SendsRequestAndReturnsResponseData_GivenSuccessfulHttpRequestWithNonNullBody()
    {
        var request = new TestRequest { TestContent = RequestString };
        var httpMethod = HttpMethod.Get;
        
        var mockResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(ResponseString)
        };

        _mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(
                    m => 
                        m.Method == httpMethod),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(mockResponse);


        _gcpDotOptionsMock.Setup(o => o.Value).Returns(_gcpDotSettings);
        
        var gcpDotNetworkDriver = new GcpDotNetworkDriver(_httpClient, _gcpDotOptionsMock.Object);

        var actual = await gcpDotNetworkDriver.SendRequestAsync("/path", httpMethod, request);

        actual.Should().BeEquivalentTo(ResponseString);
        
        _mockHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(m => 
                m.Method == httpMethod),
            ItExpr.IsAny<CancellationToken>());
    }

    private class TestRequest
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        public string? TestContent { get; set; }
    }
}
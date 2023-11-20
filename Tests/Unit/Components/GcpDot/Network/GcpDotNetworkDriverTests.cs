using System.Net;
using System.Net.Mime;
using System.Text;
using Apotheosis.Components.GCPDot.Configuration;
using Apotheosis.Components.GCPDot.Interfaces;
using Apotheosis.Components.GCPDot.Network;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Tests.Utils;

namespace Tests.Unit.Components.GcpDot.Network;

public sealed class GcpDotNetworkDriverTests : IDisposable
{
    private const string RequestString = "request123";
    private const string ResponseString = "response123";
    
    
    private readonly Mock<IOptions<GcpDotSettings>> _gcpDotOptionsMock;
    private readonly Mock<HttpMessageHandler> _mockHandler;
    private readonly IGcpDotNetworkDriver _gcpDotNetworkDriver;
    
    private readonly GcpDotSettings _gcpDotSettings = new()
    {
        BaseUrl = new Uri("https://example.com/"),
    };

    public GcpDotNetworkDriverTests()
    {
        _gcpDotOptionsMock = new Mock<IOptions<GcpDotSettings>>(MockBehavior.Strict);
        _gcpDotOptionsMock.Setup(o => o.Value).Returns(_gcpDotSettings);
        
        _mockHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        
        var httpClient = new HttpClient(_mockHandler.Object);
        _gcpDotNetworkDriver = new GcpDotNetworkDriver(httpClient, _gcpDotOptionsMock.Object);
    }

    public void Dispose()
    {
        _gcpDotOptionsMock.VerifyAll();
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
                ItExpr.Is<HttpRequestMessage>(m => 
                    m.Method == httpMethod
                    && m.Content == null),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(mockResponse);

        var actual = await _gcpDotNetworkDriver.SendRequestAsync("/path", httpMethod, null);
        
        actual.Should().BeEquivalentTo(ResponseString);
        
        _mockHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(m => 
                m.Method == httpMethod
                && m.Content == null),
            ItExpr.IsAny<CancellationToken>());
    }
    
    [Fact]
    public async Task SendRequestAsync_SendsRequestAndReturnsResponseData_GivenSuccessfulHttpRequestWithNonNullBody()
    {
        var request = new TestRequest { RequestContent = RequestString };
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
                        m.Method == httpMethod
                        && m.Content != null
                        && MatchUtils.MatchRequestContent(
                            m.Content, 
                            new StringContent(
                                JsonConvert.SerializeObject(request), 
                                Encoding.UTF8, 
                                MediaTypeNames.Application.Json))),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(mockResponse);

        var actual = await _gcpDotNetworkDriver.SendRequestAsync("/path", httpMethod, request);

        actual.Should().BeEquivalentTo(ResponseString);
        
        _mockHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(m => 
                m.Method == httpMethod
                && m.Content != null
                && MatchUtils.MatchRequestContent(
                    m.Content, 
                    new StringContent(
                        JsonConvert.SerializeObject(request), 
                        Encoding.UTF8, 
                        MediaTypeNames.Application.Json))),
            ItExpr.IsAny<CancellationToken>());
    }

    private class TestRequest
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        public string? RequestContent { get; set; }
    }
}
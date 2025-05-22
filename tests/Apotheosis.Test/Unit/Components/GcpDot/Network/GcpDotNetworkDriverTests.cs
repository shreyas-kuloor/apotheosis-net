namespace Apotheosis.Test.Unit.Components.GcpDot.Network;

public sealed class GcpDotNetworkDriverTests
{
    const string RequestString = "request123";
    const string ResponseString = "response123";


    readonly Mock<HttpMessageHandler> _mockHandler;
    readonly GcpDotNetworkDriver _gcpDotNetworkDriver;

    readonly GcpDotSettings _gcpDotSettings = new()
    {
        BaseUrl = new Uri("https://example.com/"),
    };

    public GcpDotNetworkDriverTests()
    {
        _mockHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);

        var httpClient = new HttpClient(_mockHandler.Object);
        _gcpDotNetworkDriver = new GcpDotNetworkDriver(httpClient, Options.Create(_gcpDotSettings));
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
                        && MatchUtils.MatchBasicObject(
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
                && MatchUtils.MatchBasicObject(
                    m.Content,
                    new StringContent(
                        JsonConvert.SerializeObject(request),
                        Encoding.UTF8,
                        MediaTypeNames.Application.Json))),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task SendRequestAsync_SendsRequestAndThrowsNetworkException_GivenHttpRequestWithNullBodyReturnsUnsuccessfulStatusCode()
    {
        var httpMethod = HttpMethod.Post;

        var mockResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.BadRequest,
            Content = null
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

        var actual = () => _gcpDotNetworkDriver.SendRequestAsync("/path", httpMethod, null);

        await actual.Should().ThrowAsync<GcpDotNetworkException>()
            .Where(e => e.Message == "GCP Dot service returned a non-successful status code");

        _mockHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(m =>
                m.Method == httpMethod
                && m.Content == null),
            ItExpr.IsAny<CancellationToken>());
    }

    class TestRequest
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        public string? RequestContent { get; set; }
    }
}
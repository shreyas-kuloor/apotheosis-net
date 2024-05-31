using System.Net;
using System.Net.Mime;
using System.Text;
using Apotheosis.Core.Features.TextToSpeech.Configuration;
using Apotheosis.Core.Features.TextToSpeech.Exceptions;
using Apotheosis.Core.Features.TextToSpeech.Network;
using Apotheosis.Test.Utils;
using FluentAssertions;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;

namespace Apotheosis.Test.Unit.Components.TextToSpeech.Network;

public sealed class ElevenLabsNetworkDriverTests
{
    private const string RequestString = "request123";
    private const string ResponseString = "response123";

    private readonly Mock<HttpMessageHandler> _mockHandler;
    private readonly ElevenLabsNetworkDriver _elevenLabsNetworkDriver;

    private readonly TextToSpeechSettings _textToSpeechSettings = new()
    {
        ElevenLabsBaseUrl = new Uri("https://example.com/"),
        ElevenLabsApiKey = "api-key"
    };

    public ElevenLabsNetworkDriverTests()
    {
        _mockHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);

        var httpClient = new HttpClient(_mockHandler.Object);
        _elevenLabsNetworkDriver = new ElevenLabsNetworkDriver(httpClient, Options.Create(_textToSpeechSettings));
    }

    [Fact]
    public async Task SendRequestReceiveStreamAsync_SendsRequestAndReturnsResponseStream_GivenSuccessfulHttpRequestWithNullBody()
    {
        var buffer = Encoding.UTF8.GetBytes("dummy data");
        using var stream = new MemoryStream(buffer);
        var httpMethod = HttpMethod.Post;

        var mockResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StreamContent(stream)
        };

        IEnumerable<string>? headerValues;
        _mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(m =>
                    m.Method == httpMethod
                    && m.Content == null
                    && m.Headers.TryGetValues("xi-api-key", out headerValues)
                    && headerValues.Contains(_textToSpeechSettings.ElevenLabsApiKey)),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(mockResponse);

        var actual = await _elevenLabsNetworkDriver.SendRequestReceiveStreamAsync("/path", httpMethod, null);

        using var memoryStream = new MemoryStream();
        await actual.CopyToAsync(memoryStream);
        var actualBytes = memoryStream.ToArray();
        actualBytes.Should().BeEquivalentTo(buffer);

        _mockHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(m =>
                m.Method == httpMethod
                && m.Content == null
                && m.Headers.TryGetValues("xi-api-key", out headerValues)
                && headerValues.Contains(_textToSpeechSettings.ElevenLabsApiKey)),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task SendRequestReceiveStreamAsync_SendsRequestAndReturnsResponseStream_GivenSuccessfulHttpRequestWithNonNullBody()
    {
        var request = new TestRequest { RequestContent = RequestString };
        var buffer = Encoding.UTF8.GetBytes("dummy data");
        using var stream = new MemoryStream(buffer);
        var httpMethod = HttpMethod.Post;

        var mockResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StreamContent(stream)
        };

        IEnumerable<string>? headerValues;
        _mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(m =>
                    m.Method == httpMethod
                    && m.Content != null
                    && MatchUtils.MatchBasicObject(
                        m.Content,
                        new StringContent(
                            JsonConvert.SerializeObject(request),
                            Encoding.UTF8,
                            MediaTypeNames.Application.Json))
                    && m.Headers.TryGetValues("xi-api-key", out headerValues)
                    && headerValues.Contains(_textToSpeechSettings.ElevenLabsApiKey)),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(mockResponse);

        var actual = await _elevenLabsNetworkDriver.SendRequestReceiveStreamAsync("/path", httpMethod, request);

        using var memoryStream = new MemoryStream();
        await actual.CopyToAsync(memoryStream);
        var actualBytes = memoryStream.ToArray();
        actualBytes.Should().BeEquivalentTo(buffer);

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
                        MediaTypeNames.Application.Json))
                && m.Headers.TryGetValues("xi-api-key", out headerValues)
                && headerValues.Contains(_textToSpeechSettings.ElevenLabsApiKey)),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task SendRequestReceiveStreamAsync_SendsRequestAndThrowsNetworkException_GivenHttpRequestWithNullBodyReturnsUnsuccessfulStatusCode()
    {
        var httpMethod = HttpMethod.Post;

        var mockResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.BadRequest,
            Content = null
        };

        IEnumerable<string>? headerValues;
        _mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(m =>
                    m.Method == httpMethod
                    && m.Content == null
                    && m.Headers.TryGetValues("xi-api-key", out headerValues)
                    && headerValues.Contains(_textToSpeechSettings.ElevenLabsApiKey)),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(mockResponse);

        var actual = () => _elevenLabsNetworkDriver.SendRequestAsync<TestResponse>("/path", httpMethod, null);

        await actual.Should().ThrowAsync<TextToSpeechNetworkException>()
            .Where(e => e.Message == "Eleven Labs returned a non-successful status code");

        _mockHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(m =>
                m.Method == httpMethod
                && m.Content == null
                && m.Headers.TryGetValues("xi-api-key", out headerValues)
                && headerValues.Contains(_textToSpeechSettings.ElevenLabsApiKey)),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task SendRequestAsync_SendsRequestAndReturnsResponseData_GivenSuccessfulHttpRequestWithNullBody()
    {
        var httpMethod = HttpMethod.Post;
        var response = new TestResponse { ResponseContent = ResponseString };

        var mockResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonConvert.SerializeObject(response))
        };

        IEnumerable<string>? headerValues;
        _mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(m =>
                    m.Method == httpMethod
                    && m.Content == null
                    && m.Headers.TryGetValues("xi-api-key", out headerValues)
                    && headerValues.Contains(_textToSpeechSettings.ElevenLabsApiKey)),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(mockResponse);

        var actual = await _elevenLabsNetworkDriver.SendRequestAsync<TestResponse>("/path", httpMethod, null);

        actual.Should().BeEquivalentTo(response);

        _mockHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(m =>
                m.Method == httpMethod
                && m.Content == null
                && m.Headers.TryGetValues("xi-api-key", out headerValues)
                && headerValues.Contains(_textToSpeechSettings.ElevenLabsApiKey)),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task SendRequestAsync_SendsRequestAndReturnsResponseData_GivenSuccessfulHttpRequestWithNonNullBody()
    {
        var httpMethod = HttpMethod.Post;
        var request = new TestRequest { RequestContent = RequestString };
        var response = new TestResponse { ResponseContent = ResponseString };

        var mockResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonConvert.SerializeObject(response))
        };

        IEnumerable<string>? headerValues;
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
                                MediaTypeNames.Application.Json))
                        && m.Headers.TryGetValues("xi-api-key", out headerValues)
                        && headerValues.Contains(_textToSpeechSettings.ElevenLabsApiKey)),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(mockResponse);

        var actual = await _elevenLabsNetworkDriver.SendRequestAsync<TestResponse>("/path", httpMethod, request);

        actual.Should().BeEquivalentTo(response);

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
                        MediaTypeNames.Application.Json))
                && m.Headers.TryGetValues("xi-api-key", out headerValues)
                && headerValues.Contains(_textToSpeechSettings.ElevenLabsApiKey)),
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


        var actual = () => _elevenLabsNetworkDriver.SendRequestAsync<TestResponse>("/path", httpMethod, null);

        await actual.Should().ThrowAsync<TextToSpeechNetworkException>()
            .Where(e => e.Message == "Eleven Labs returned a non-successful status code");

        _mockHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(m =>
                m.Method == httpMethod
                && m.Content == null),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task SendRequestAsync_SendsRequestAndThrowsNetworkException_GivenHttpRequestWithNullBodyReturnsNonJsonResponse()
    {
        var httpMethod = HttpMethod.Post;

        var mockResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(string.Empty)
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


        var actual = () => _elevenLabsNetworkDriver.SendRequestAsync<TestResponse>("/path", httpMethod, null);

        await actual.Should().ThrowAsync<TextToSpeechNetworkException>()
            .Where(e => e.Message == "An error occured while sending the Eleven Labs network request.");

        _mockHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(m =>
                m.Method == httpMethod
                && m.Content == null),
            ItExpr.IsAny<CancellationToken>());
    }

    private class TestRequest
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        public string? RequestContent { get; set; }
    }

    private class TestResponse
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        public string? ResponseContent { get; set; }
    }
}
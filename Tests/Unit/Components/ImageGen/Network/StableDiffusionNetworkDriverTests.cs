using System.Net;
using System.Net.Mime;
using System.Text;
using Apotheosis.Components.ImageGen.Configuration;
using Apotheosis.Components.ImageGen.Exceptions;
using Apotheosis.Components.ImageGen.Interfaces;
using Apotheosis.Components.ImageGen.Network;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Tests.Utils;

namespace Tests.Unit.Components.ImageGen.Network;

public sealed class StableDiffusionNetworkDriverTests : IDisposable
{
    private const string RequestString = "request123";
    private const string ResponseString = "response123";
    
    private readonly Mock<IOptions<ImageGenSettings>> _imageGenOptionsMock;
    private readonly Mock<HttpMessageHandler> _mockHandler;
    private readonly IImageGenNetworkDriver _imageGenNetworkDriver;
    
    private readonly ImageGenSettings _imageGenSettings = new()
    {
        StableDiffusionBaseUrl = new Uri("https://example.com/"),
        StableDiffusionSamplingSteps = 50
    };

    public StableDiffusionNetworkDriverTests()
    {
        _imageGenOptionsMock = new Mock<IOptions<ImageGenSettings>>(MockBehavior.Strict);
        _imageGenOptionsMock.Setup(o => o.Value).Returns(_imageGenSettings);
        
        _mockHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        
        var httpClient = new HttpClient(_mockHandler.Object);
        _imageGenNetworkDriver = new StableDiffusionNetworkDriver(httpClient, _imageGenOptionsMock.Object);
    }

    public void Dispose()
    {
        _imageGenOptionsMock.VerifyAll();
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

        _mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(m => 
                    m.Method == httpMethod
                    && m.Content == null),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(mockResponse);

        var actual = await _imageGenNetworkDriver.SendRequestAsync<TestResponse>("/path", httpMethod, null);
        
        actual.Should().BeEquivalentTo(response);
        
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
        var httpMethod = HttpMethod.Post;
        var request = new TestRequest { RequestContent = RequestString };
        var response = new TestResponse { ResponseContent = ResponseString };
        
        var mockResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonConvert.SerializeObject(response))
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

        var actual = await _imageGenNetworkDriver.SendRequestAsync<TestResponse>("/path", httpMethod, request);

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


        var actual = () => _imageGenNetworkDriver.SendRequestAsync<TestResponse>("/path", httpMethod, null);

        await actual.Should().ThrowAsync<ImageGenNetworkException>()
            .Where(e => e.Message == "Stable Diffusion returned a non-successful status code");
        
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


        var actual = () => _imageGenNetworkDriver.SendRequestAsync<TestResponse>("/path", httpMethod, null);

        await actual.Should().ThrowAsync<ImageGenNetworkException>()
            .Where(e => e.Message == "An error occured while sending the Stable Diffusion network request.");
        
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
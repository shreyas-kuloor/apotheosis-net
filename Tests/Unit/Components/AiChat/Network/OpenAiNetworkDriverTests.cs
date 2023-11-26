﻿using System.Net;
using System.Net.Mime;
using System.Text;
using Apotheosis.Components.AiChat.Configuration;
using Apotheosis.Components.AiChat.Exceptions;
using Apotheosis.Components.AiChat.Interfaces;
using Apotheosis.Components.AiChat.Network;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Tests.Utils;

namespace Tests.Unit.Components.AiChat.Network;

public sealed class OpenAiNetworkDriverTests : IDisposable
{
    private const string RequestString = "request123";
    private const string ResponseString = "response123";
    
    private readonly Mock<IOptions<AiChatSettings>> _aiChatOptionsMock;
    private readonly Mock<HttpMessageHandler> _mockHandler;
    private readonly IAiChatNetworkDriver _aiChatNetworkDriver;
    
    private readonly AiChatSettings _aiChatSettings = new()
    {
        OpenAiBaseUrl = new Uri("https://example.com/"),
        OpenAiApiKey = "example-api-key"
    };

    public OpenAiNetworkDriverTests()
    {
        _aiChatOptionsMock = new Mock<IOptions<AiChatSettings>>(MockBehavior.Strict);
        _aiChatOptionsMock.Setup(o => o.Value).Returns(_aiChatSettings);
        
        _mockHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        
        var httpClient = new HttpClient(_mockHandler.Object);
        _aiChatNetworkDriver = new OpenAiNetworkDriver(httpClient, _aiChatOptionsMock.Object);
    }

    public void Dispose()
    {
        _aiChatOptionsMock.VerifyAll();
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
                    && m.Headers.TryGetValues("Authorization", out headerValues)
                    && headerValues.Contains($"Bearer {_aiChatSettings.OpenAiApiKey}")
                    && m.Content == null),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(mockResponse);

        var actual = await _aiChatNetworkDriver.SendRequestAsync<TestResponse>("/path", httpMethod, null);
        
        actual.Should().BeEquivalentTo(response);
        
        _mockHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(m => 
                m.Method == httpMethod
                && m.Headers.TryGetValues("Authorization", out headerValues)
                && headerValues.Contains($"Bearer {_aiChatSettings.OpenAiApiKey}")
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

        IEnumerable<string>? headerValues;
        _mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(
                    m => 
                        m.Method == httpMethod
                        && m.Headers.TryGetValues("Authorization", out headerValues)
                        && headerValues.Contains($"Bearer {_aiChatSettings.OpenAiApiKey}")
                        && m.Content != null
                        && MatchUtils.MatchBasicObject(
                            m.Content, 
                            new StringContent(
                                JsonConvert.SerializeObject(request), 
                                Encoding.UTF8, 
                                MediaTypeNames.Application.Json))),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(mockResponse);

        var actual = await _aiChatNetworkDriver.SendRequestAsync<TestResponse>("/path", httpMethod, request);

        actual.Should().BeEquivalentTo(response);
        
        _mockHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(m => 
                m.Method == httpMethod
                && m.Headers.TryGetValues("Authorization", out headerValues)
                && headerValues.Contains($"Bearer {_aiChatSettings.OpenAiApiKey}")
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

        IEnumerable<string>? headerValues;
        _mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(m => 
                    m.Method == httpMethod
                    && m.Headers.TryGetValues("Authorization", out headerValues)
                    && headerValues.Contains($"Bearer {_aiChatSettings.OpenAiApiKey}")
                    && m.Content == null),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(mockResponse);


        var actual = () => _aiChatNetworkDriver.SendRequestAsync<TestResponse>("/path", httpMethod, null);

        await actual.Should().ThrowAsync<AiChatNetworkException>()
            .Where(e => 
                e.Message == "OpenAI returned a non-successful status code"
                && e.Reason == AiChatNetworkException.ErrorReason.Unsuccessful);
        
        _mockHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(m => 
                m.Method == httpMethod
                && m.Headers.TryGetValues("Authorization", out headerValues)
                && headerValues.Contains($"Bearer {_aiChatSettings.OpenAiApiKey}")
                && m.Content == null),
            ItExpr.IsAny<CancellationToken>());
    }
    
    [Fact]
    public async Task SendRequestAsync_SendsRequestAndThrowsNetworkExceptionWithTokenQuotaReachedErrorReason_GivenHttpRequestWithNullBodyReturnsTooManyRequestsCode()
    {
        var httpMethod = HttpMethod.Post;
        
        var mockResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.TooManyRequests,
            Content = null
        };

        IEnumerable<string>? headerValues;
        _mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(m => 
                    m.Method == httpMethod
                    && m.Headers.TryGetValues("Authorization", out headerValues)
                    && headerValues.Contains($"Bearer {_aiChatSettings.OpenAiApiKey}")
                    && m.Content == null),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(mockResponse);


        var actual = () => _aiChatNetworkDriver.SendRequestAsync<TestResponse>("/path", httpMethod, null);

        await actual.Should().ThrowAsync<AiChatNetworkException>()
            .Where(e => 
                e.Message == "OpenAI returned a too many requests error code"
                && e.Reason == AiChatNetworkException.ErrorReason.TokenQuotaReached);
        
        _mockHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(m => 
                m.Method == httpMethod
                && m.Headers.TryGetValues("Authorization", out headerValues)
                && headerValues.Contains($"Bearer {_aiChatSettings.OpenAiApiKey}")
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

        IEnumerable<string>? headerValues;
        _mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(m => 
                    m.Method == httpMethod
                    && m.Headers.TryGetValues("Authorization", out headerValues)
                    && headerValues.Contains($"Bearer {_aiChatSettings.OpenAiApiKey}")
                    && m.Content == null),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(mockResponse);


        var actual = () => _aiChatNetworkDriver.SendRequestAsync<TestResponse>("/path", httpMethod, null);

        await actual.Should().ThrowAsync<AiChatNetworkException>()
            .Where(e => 
                e.Message == "An error occured while sending the OpenAI network request"
                && e.Reason == AiChatNetworkException.ErrorReason.Unknown);
        
        _mockHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(m => 
                m.Method == httpMethod
                && m.Headers.TryGetValues("Authorization", out headerValues)
                && headerValues.Contains($"Bearer {_aiChatSettings.OpenAiApiKey}")
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
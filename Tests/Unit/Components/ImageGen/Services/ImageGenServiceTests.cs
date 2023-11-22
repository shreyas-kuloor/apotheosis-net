using Apotheosis.Components.ImageGen.Configuration;
using Apotheosis.Components.ImageGen.Interfaces;
using Apotheosis.Components.ImageGen.Models;
using Apotheosis.Components.ImageGen.Services;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Tests.Utils;

namespace Tests.Unit.Components.ImageGen.Services;

public sealed class ImageGenServiceTests : IDisposable
{
    private readonly Mock<IImageGenNetworkDriver> _imageGenNetworkDriverMock;
    private readonly Mock<IOptions<ImageGenSettings>> _imageGenOptionsMock;
    private readonly IImageGenService _imageGenService;
    
    private readonly ImageGenSettings _imageGenSettings = new()
    {
        StableDiffusionBaseUrl = new Uri("https://example.com/"),
        StableDiffusionSamplingSteps = 50
    };

    public ImageGenServiceTests()
    {
        _imageGenNetworkDriverMock = new Mock<IImageGenNetworkDriver>(MockBehavior.Strict);
        _imageGenOptionsMock = new Mock<IOptions<ImageGenSettings>>(MockBehavior.Strict);
        _imageGenOptionsMock.Setup(o => o.Value).Returns(_imageGenSettings);

        _imageGenService = new ImageGenService(_imageGenNetworkDriverMock.Object, _imageGenOptionsMock.Object);
    }

    public void Dispose()
    {
        _imageGenNetworkDriverMock.VerifyAll();
        _imageGenOptionsMock.VerifyAll();
    }
    
    [Fact]
    public async Task GenerateImageBase64FromPromptAsync_ReturnsBase64Image_GivenPromptAndSteps()
    {
        const string prompt = "prompt";
        const int samplingSteps = 100;
        const string imageString = "abc123";
        
        var expectedImageGenRequest = new StableDiffusionTextRequest
        {
            Prompt = prompt,
            Steps = samplingSteps,
        };

        var imageGenResponse = new StableDiffusionResponse
        {
            Images = new List<string>
            {
                imageString
            },
        };

        _imageGenNetworkDriverMock.Setup(d =>
            d.SendRequestAsync<StableDiffusionResponse>(
                "/txt2img", 
                HttpMethod.Post, 
                Match.Create<StableDiffusionTextRequest>(
                    r => MatchUtils.MatchBasicObject(r, expectedImageGenRequest))))
            .ReturnsAsync(imageGenResponse);

        var actual = await _imageGenService.GenerateImageBase64FromPromptAsync(prompt, samplingSteps);

        actual.Should().BeEquivalentTo(imageString);
    }
    
    [Fact]
    public async Task GenerateImageBase64FromPromptAsync_ReturnsBase64Image_GivenPromptAndNullSteps()
    {
        const string prompt = "prompt";
        const string imageString = "abc123";
        
        var expectedImageGenRequest = new StableDiffusionTextRequest
        {
            Prompt = prompt,
            Steps = 50,
        };

        var imageGenResponse = new StableDiffusionResponse
        {
            Images = new List<string>
            {
                imageString
            },
        };

        _imageGenNetworkDriverMock.Setup(d =>
                d.SendRequestAsync<StableDiffusionResponse>(
                    "/txt2img", 
                    HttpMethod.Post, 
                    Match.Create<StableDiffusionTextRequest>(
                        r => MatchUtils.MatchBasicObject(r, expectedImageGenRequest))))
            .ReturnsAsync(imageGenResponse);

        var actual = await _imageGenService.GenerateImageBase64FromPromptAsync(prompt, null);

        actual.Should().BeEquivalentTo(imageString);
    }
}
using Apotheosis.Core.Features.ImageGen.Configuration;
using Apotheosis.Core.Features.ImageGen.Interfaces;
using Apotheosis.Core.Features.ImageGen.Models;
using Apotheosis.Core.Features.ImageGen.Services;
using Apotheosis.Test.Utils;
using FluentAssertions;
using Moq;

namespace Apotheosis.Test.Unit.Features.ImageGen.Services;

public sealed class ImageGenServiceTests : IDisposable
{
    private readonly Mock<IImageGenNetworkDriver> _imageGenNetworkDriverMock;
    private readonly ImageGenService _imageGenService;
    
    private readonly ImageGenSettings _imageGenSettings = new()
    {
        StableDiffusionBaseUrl = new Uri("https://example.com/"),
        StableDiffusionSamplingSteps = 50
    };

    public ImageGenServiceTests()
    {
        _imageGenNetworkDriverMock = new Mock<IImageGenNetworkDriver>(MockBehavior.Strict);

        _imageGenService = new ImageGenService(_imageGenNetworkDriverMock.Object, Options.Create(_imageGenSettings));
    }

    public void Dispose()
    {
        _imageGenNetworkDriverMock.VerifyAll();
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
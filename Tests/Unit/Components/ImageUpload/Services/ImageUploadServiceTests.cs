using Apotheosis.Components.ImageUpload.Interfaces;
using Apotheosis.Components.ImageUpload.Models;
using Apotheosis.Components.ImageUpload.Services;
using FluentAssertions;
using Moq;

namespace Tests.Unit.Components.ImageUpload.Services;

public sealed class ImageUploadServiceTests : IDisposable
{
    private readonly Mock<IImageUploadNetworkDriver> _imageUploadNetworkDriverMock;
    private readonly IImageUploadService _imageUploadService;

    public ImageUploadServiceTests()
    {
        _imageUploadNetworkDriverMock = new Mock<IImageUploadNetworkDriver>(MockBehavior.Strict);
        _imageUploadService = new ImageUploadService(_imageUploadNetworkDriverMock.Object);
    }
    
    public void Dispose()
    {
        _imageUploadNetworkDriverMock.VerifyAll();
    }

    [Fact]
    public async Task UploadImageAsync_ReturnsImageUri_GivenBase64ImageString()
    {
        const string base64ImageString = "abc";
        var imageUploadUri = new Uri("https://example.com/image");

        var imageUploadResponse = new ImgurUploadResponse
        {
            Data = new ImgurUploadDataResponse
            {
                Link = imageUploadUri
            }
        };

        _imageUploadNetworkDriverMock.Setup(d =>
                d.SendRequestAsync<ImgurUploadRequest, ImgurUploadResponse>(
                    "/3/image",
                    HttpMethod.Post,
                    Match.Create<ImgurUploadRequest>(r => r.Image == base64ImageString)))
            .ReturnsAsync(imageUploadResponse);

        var actual = await _imageUploadService.UploadImageAsync(base64ImageString);

        actual.Should().BeEquivalentTo(imageUploadUri);
    }
    
    
}
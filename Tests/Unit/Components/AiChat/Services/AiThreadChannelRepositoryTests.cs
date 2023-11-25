using Apotheosis.Components.AiChat.Configuration;
using Apotheosis.Components.AiChat.Exceptions;
using Apotheosis.Components.AiChat.Interfaces;
using Apotheosis.Components.AiChat.Models;
using Apotheosis.Components.AiChat.Services;
using Apotheosis.Components.DateTime.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;

namespace Tests.Unit.Components.AiChat.Services;

public sealed class AiThreadChannelRepositoryTests : IDisposable
{
    private readonly Mock<IOptions<AiChatSettings>> _aiChatOptionsMock;
    private readonly Mock<IDateTimeService> _dateTimeServiceMock;
    private readonly IAiThreadChannelRepository _aiThreadChannelRepository;
    
    private readonly AiChatSettings _aiChatSettings = new()
    {
        ThreadExpirationMinutes = 1440
    };

    public AiThreadChannelRepositoryTests()
    {
        _aiChatOptionsMock = new Mock<IOptions<AiChatSettings>>(MockBehavior.Strict);
        _aiChatOptionsMock.Setup(o => o.Value).Returns(_aiChatSettings);
        _dateTimeServiceMock = new Mock<IDateTimeService>(MockBehavior.Strict);

        _aiThreadChannelRepository = new AiThreadChannelRepository(_aiChatOptionsMock.Object, _dateTimeServiceMock.Object);
    }

    public void Dispose()
    {
        _aiChatOptionsMock.VerifyAll();
        _dateTimeServiceMock.VerifyAll();
    }

    [Fact]
    public void StoreThreadChannel_DoesNotThrowException_GivenSuccessfulAdd()
    {
        const ulong threadId = 123;
        const string createDateTimeOffset = "2023-11-25T18:00:00+0000";
        const string expiredDateTimeOffset = "2023-11-26T18:00:00+0000";

        _dateTimeServiceMock.SetupGet(s => s.UtcNow).Returns(DateTimeOffset.Parse(createDateTimeOffset));
        var exception = Record.Exception(() => _aiThreadChannelRepository.StoreThreadChannel(threadId));
        
        exception.Should().BeNull();
        
        var existingThreadChannels = _aiThreadChannelRepository.GetStoredThreadChannels();

        existingThreadChannels.Should().ContainEquivalentOf(new ThreadChannelDto
        {
            ThreadId = threadId,
            Expiration = DateTimeOffset.Parse(expiredDateTimeOffset)
        });
    }
    
    [Fact]
    public void StoreThreadChannel_ThrowsAiThreadChannelStoreException_GivenUnsuccessfulAdd()
    {
        const ulong threadId = 123;
        
        _dateTimeServiceMock.SetupGet(s => s.UtcNow).Returns(DateTimeOffset.Parse("2023-11-25T18:00:00+0000"));
        _dateTimeServiceMock.SetupGet(s => s.UtcNow).Returns(DateTimeOffset.Parse("2023-11-25T18:00:10+0000"));
       _aiThreadChannelRepository.StoreThreadChannel(threadId);

       var actual = () => _aiThreadChannelRepository.StoreThreadChannel(threadId);

       actual.Should().Throw<AiThreadChannelStoreException>()
           .Where(e => e.Message == "An error occurred while trying to store a thread channel");
    }
    
    [Fact]
    public void ClearExpiredThreadChannels_DoesNotThrowException_GivenSuccessfulClear()
    {
        const ulong threadId1 = 123;
        const ulong threadId2 = 124;
        const string dateTimeOffset1 = "2023-11-25T18:00:00+0000";
        const string dateTimeOffset2 = "2023-11-26T18:10:00+0000";
        const string expirationDateTimeOffset1 = "2023-11-26T18:00:00+0000";
        const string expirationDateTimeOffset2 = "2023-11-27T18:10:00+0000";
        const string clearDateTimeOffset = "2023-11-27T18:01:00+0000";
        const string retrievalDateTimeOffset = "2023-11-27T18:02:00+0000";

        _dateTimeServiceMock.SetupSequence(s => s.UtcNow)
            .Returns(DateTimeOffset.Parse(dateTimeOffset1))
            .Returns(DateTimeOffset.Parse(dateTimeOffset2))
            .Returns(DateTimeOffset.Parse(clearDateTimeOffset))
            .Returns(DateTimeOffset.Parse(retrievalDateTimeOffset));
        
        _aiThreadChannelRepository.StoreThreadChannel(threadId1);
        _aiThreadChannelRepository.StoreThreadChannel(threadId2);
        
        var exception = Record.Exception(() => _aiThreadChannelRepository.ClearExpiredThreadChannels());
        
        exception.Should().BeNull();

        var existingThreadChannels = _aiThreadChannelRepository.GetStoredThreadChannels();

        var threadChannelDtos = existingThreadChannels.ToList();
        threadChannelDtos.Should().NotContainEquivalentOf(new ThreadChannelDto
        {
            ThreadId = threadId1,
            Expiration = DateTimeOffset.Parse(expirationDateTimeOffset1)
        });

        threadChannelDtos.Should().ContainEquivalentOf(new ThreadChannelDto
        {
            ThreadId = threadId2,
            Expiration = DateTimeOffset.Parse(expirationDateTimeOffset2)
        });
    }
    
    [Fact]
    public void GetStoredThreadChannels_ReturnsStoredThreadChannels_GivenExistingThreadChannels()
    {
        const ulong threadId1 = 123;
        const ulong threadId2 = 124;
        const string dateTimeOffset1 = "2023-11-25T18:00:00+0000";
        const string dateTimeOffset2 = "2023-11-26T18:10:00+0000";
        const string expirationDateTimeOffset1 = "2023-11-26T18:00:00+0000";
        const string expirationDateTimeOffset2 = "2023-11-27T18:10:00+0000";
        const string retrievalDateTimeOffset = "2023-11-27T18:01:00+0000";

        _dateTimeServiceMock.SetupSequence(s => s.UtcNow)
            .Returns(DateTimeOffset.Parse(dateTimeOffset1))
            .Returns(DateTimeOffset.Parse(dateTimeOffset2))
            .Returns(DateTimeOffset.Parse(retrievalDateTimeOffset));
        
        _aiThreadChannelRepository.StoreThreadChannel(threadId1);
        _aiThreadChannelRepository.StoreThreadChannel(threadId2);
        
        var existingThreadChannels = _aiThreadChannelRepository.GetStoredThreadChannels();

        var threadChannelDtos = existingThreadChannels.ToList();
        threadChannelDtos.Should().NotContainEquivalentOf(new ThreadChannelDto
        {
            ThreadId = threadId1,
            Expiration = DateTimeOffset.Parse(expirationDateTimeOffset1)
        });

        threadChannelDtos.Should().ContainEquivalentOf(new ThreadChannelDto
        {
            ThreadId = threadId2,
            Expiration = DateTimeOffset.Parse(expirationDateTimeOffset2)
        });
    }
}
using Apotheosis.Core.Features.AiChat.Models;
using Apotheosis.Core.Features.AiChat.Exceptions;
using Apotheosis.Core.Features.AiChat.Services;
using Apotheosis.Core.Features.DateTime.Interfaces;
using FluentAssertions;
using Moq;

namespace Apotheosis.Test.Unit.Features.AiChat.Services;

public sealed class AiThreadChannelRepositoryTests : IDisposable
{
    private readonly Mock<IDateTimeService> _dateTimeServiceMock;
    private readonly AiThreadChannelRepository _aiThreadChannelRepository;

    public AiThreadChannelRepositoryTests()
    {
        _dateTimeServiceMock = new Mock<IDateTimeService>(MockBehavior.Strict);

        _aiThreadChannelRepository = new AiThreadChannelRepository(_dateTimeServiceMock.Object);
    }

    public void Dispose()
    {
        _dateTimeServiceMock.VerifyAll();
    }

    [Fact]
    public void StoreThreadChannel_DoesNotThrowException_GivenSuccessfulAdd()
    {
        const ulong threadId = 123;
        const string dateTimeOffset = "2023-11-25T18:00:00+0000";
        const string expiredDateTimeOffset = "2023-11-26T18:00:00+0000";
        const string initialMessage = "example";
        
        var threadDto = new ThreadChannelDto
        {
            ThreadId = threadId,
            Expiration = DateTimeOffset.Parse(expiredDateTimeOffset),
            InitialMessageContent = initialMessage
        };

        _dateTimeServiceMock.SetupGet(d => d.UtcNow).Returns(DateTimeOffset.Parse(dateTimeOffset));
        
        var exception = Record.Exception(() => _aiThreadChannelRepository.StoreThreadChannel(threadDto));
        
        exception.Should().BeNull();
        
        var existingThreadChannels = _aiThreadChannelRepository.GetStoredActiveThreadChannels();

        existingThreadChannels.Should().Contain(threadDto);
    }
    
    [Fact]
    public void StoreThreadChannel_ThrowsAiThreadChannelStoreException_GivenUnsuccessfulAdd()
    {
        const ulong threadId = 123;
        const string dateTimeOffset1 = "2023-11-25T18:00:00+0000";
        const string dateTimeOffset2 = "2023-11-25T18:00:10+0000";
        const string initialMessage = "example";
        
        var threadDto1 = new ThreadChannelDto
        {
            ThreadId = threadId,
            Expiration = DateTimeOffset.Parse(dateTimeOffset1),
            InitialMessageContent = initialMessage
        };
        
        var threadDto2 = new ThreadChannelDto
        {
            ThreadId = threadId,
            Expiration = DateTimeOffset.Parse(dateTimeOffset2),
            InitialMessageContent = initialMessage
        };
        
       _aiThreadChannelRepository.StoreThreadChannel(threadDto1);

       var actual = () => _aiThreadChannelRepository.StoreThreadChannel(threadDto2);

       actual.Should().Throw<AiThreadChannelStoreException>()
           .Where(e => e.Message == "An error occurred while trying to store a thread channel");
    }
    
    [Fact]
    public void ClearExpiredThreadChannels_DoesNotThrowException_GivenSuccessfulClear()
    {
        const ulong threadId1 = 123;
        const ulong threadId2 = 124;
        const string expirationDateTimeOffset1 = "2023-11-26T18:00:00+0000";
        const string expirationDateTimeOffset2 = "2023-11-27T18:10:00+0000";
        const string clearDateTimeOffset = "2023-11-27T18:01:00+0000";
        const string retrievalDateTimeOffset = "2023-11-27T18:02:00+0000";
        const string initialMessage = "example";

        _dateTimeServiceMock.SetupSequence(s => s.UtcNow)
            .Returns(DateTimeOffset.Parse(clearDateTimeOffset))
            .Returns(DateTimeOffset.Parse(retrievalDateTimeOffset));
        
        var threadDto1 = new ThreadChannelDto
        {
            ThreadId = threadId1,
            Expiration = DateTimeOffset.Parse(expirationDateTimeOffset1),
            InitialMessageContent = initialMessage
        };
        
        var threadDto2 = new ThreadChannelDto
        {
            ThreadId = threadId2,
            Expiration = DateTimeOffset.Parse(expirationDateTimeOffset2),
            InitialMessageContent = initialMessage
        };
        
        _aiThreadChannelRepository.StoreThreadChannel(threadDto1);
        _aiThreadChannelRepository.StoreThreadChannel(threadDto2);
        
        var exception = Record.Exception(() => _aiThreadChannelRepository.ClearExpiredThreadChannels());
        
        exception.Should().BeNull();

        var existingThreadChannels = _aiThreadChannelRepository.GetStoredActiveThreadChannels();

        var threadChannelDtos = existingThreadChannels.ToList();
        threadChannelDtos.Should().NotContain(threadDto1);

        threadChannelDtos.Should().Contain(threadDto2);
    }
    
    [Fact]
    public void GetStoredThreadChannels_ReturnsStoredThreadChannels_GivenExistingThreadChannels()
    {
        const ulong threadId1 = 123;
        const ulong threadId2 = 124;
        const string expirationDateTimeOffset1 = "2023-11-26T18:00:00+0000";
        const string expirationDateTimeOffset2 = "2023-11-27T18:10:00+0000";
        const string retrievalDateTimeOffset = "2023-11-27T18:01:00+0000";
        const string initialMessage = "example";

        _dateTimeServiceMock.SetupSequence(s => s.UtcNow)
            .Returns(DateTimeOffset.Parse(retrievalDateTimeOffset));
        
        var threadDto1 = new ThreadChannelDto
        {
            ThreadId = threadId1,
            Expiration = DateTimeOffset.Parse(expirationDateTimeOffset1),
            InitialMessageContent = initialMessage
        };
        
        var threadDto2 = new ThreadChannelDto
        {
            ThreadId = threadId2,
            Expiration = DateTimeOffset.Parse(expirationDateTimeOffset2),
            InitialMessageContent = initialMessage
        };
        
        _aiThreadChannelRepository.StoreThreadChannel(threadDto1);
        _aiThreadChannelRepository.StoreThreadChannel(threadDto2);
        
        var existingThreadChannels = _aiThreadChannelRepository.GetStoredActiveThreadChannels();

        var threadChannelDtos = existingThreadChannels.ToList();
        threadChannelDtos.Should().NotContain(threadDto1);

        threadChannelDtos.Should().Contain(threadDto2);
    }
}
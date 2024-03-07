using System.Collections.Concurrent;
using Apotheosis.Core.Features.AiChat.Exceptions;
using Apotheosis.Core.Features.AiChat.Interfaces;
using Apotheosis.Core.Features.AiChat.Models;
using Apotheosis.Core.Features.DateTime.Interfaces;

namespace Apotheosis.Core.Features.AiChat.Services;

[Singleton]
public sealed class AiThreadChannelRepository(IDateTimeService dateTimeService) : IAiThreadChannelRepository
{
    private readonly ConcurrentDictionary<ulong, ThreadChannelDto> _aiThreadChannelDictionary = new();

    public void StoreThreadChannel(ThreadChannelDto threadChannelDto)
    {
        var success = _aiThreadChannelDictionary.TryAdd(threadChannelDto.ThreadId, threadChannelDto);

        if (!success)
        {
            throw new AiThreadChannelStoreException("An error occurred while trying to store a thread channel");
        }
    }

    public void ClearExpiredThreadChannels()
    {
        var nowDateTimeOffset = dateTimeService.UtcNow;
        var expiredThreadChannels = _aiThreadChannelDictionary.Where(tc => tc.Value.Expiration < nowDateTimeOffset);

        var successes = expiredThreadChannels.Select(tc => _aiThreadChannelDictionary.TryRemove(tc));

        if (successes.Any(s => !s))
        {
            throw new AiThreadChannelStoreException("An error occurred while trying to remove expired thread channels.");
        }
    }

    public IEnumerable<ThreadChannelDto> GetStoredActiveThreadChannels()
    {
        var nowDateTimeOffset = dateTimeService.UtcNow;
        var storedThreadChannels = _aiThreadChannelDictionary
            .Where(tc => tc.Value.Expiration >= nowDateTimeOffset)
            .Select(tc => tc.Value);

        return storedThreadChannels;
    }
}
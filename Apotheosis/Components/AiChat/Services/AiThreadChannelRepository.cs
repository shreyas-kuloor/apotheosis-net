using System.Collections.Concurrent;
using Apotheosis.Components.AiChat.Configuration;
using Apotheosis.Components.AiChat.Exceptions;
using Apotheosis.Components.AiChat.Interfaces;
using Apotheosis.Components.AiChat.Models;
using Apotheosis.Components.DateTime.Interfaces;
using Microsoft.Extensions.Options;

namespace Apotheosis.Components.AiChat.Services;

public sealed class AiThreadChannelRepository : IAiThreadChannelRepository
{
    private readonly ConcurrentDictionary<ulong, DateTimeOffset> _aiThreadChannelDictionary = new();
    private readonly AiChatSettings _aiChatSettings;
    private readonly IDateTimeService _dateTimeService;

    public AiThreadChannelRepository(IOptions<AiChatSettings> aiChatOptions, IDateTimeService dateTimeService)
    {
        _aiChatSettings = aiChatOptions.Value;
        _dateTimeService = dateTimeService;
    }

    public void StoreThreadChannel(ulong threadId)
    {
        var success = _aiThreadChannelDictionary.TryAdd(threadId, _dateTimeService.UtcNow.AddMinutes(_aiChatSettings.ThreadExpirationMinutes));

        if (!success)
        {
            throw new AiThreadChannelStoreException("An error occurred while trying to store a thread channel");
        }
    }

    public void ClearExpiredThreadChannels()
    {
        var nowDateTimeOffset = _dateTimeService.UtcNow;
        var expiredThreadChannels = _aiThreadChannelDictionary.Where(tc => tc.Value < nowDateTimeOffset);

        var successes = expiredThreadChannels.Select(tc => _aiThreadChannelDictionary.TryRemove(tc));

        if (successes.Any(s => !s))
        {
            throw new AiThreadChannelStoreException("An error occurred while trying to remove expired thread channels.");
        }
    }

    public IEnumerable<ThreadChannelDto> GetStoredThreadChannels()
    {
        var nowDateTimeOffset = _dateTimeService.UtcNow;
        var storedThreadChannels = _aiThreadChannelDictionary
            .Where(tc => tc.Value >= nowDateTimeOffset)
            .Select(tc => new ThreadChannelDto
            {
                ThreadId = tc.Key,
                Expiration = tc.Value
            });

        return storedThreadChannels;
    }
}
﻿using System.Collections.Concurrent;
using Apotheosis.Server.Features.Audio.Exceptions;
using NetCord.Gateway.Voice;

namespace Apotheosis.Server.Features.Audio.Services;

[Singleton]
public sealed class VoiceClientService
{
    readonly ConcurrentDictionary<ulong, VoiceClient> _voiceClientDictionary = new();

    public void StoreVoiceClient(ulong voiceChannelId, VoiceClient voiceClient)
    {
        var success = _voiceClientDictionary.TryAdd(voiceChannelId, voiceClient);

        if (!success)
        {
            throw new VoiceChannelStoreException("Voice channel already exists for guild ID.");
        }
    }

    public bool TryGetVoiceClient(ulong voiceChannelId, out VoiceClient? voiceClient)
    {
        var retrieved = _voiceClientDictionary.TryGetValue(voiceChannelId, out var dictVoiceClient);
        voiceClient = dictVoiceClient;
        return retrieved;
    }

    public bool TryRemoveVoiceClient(ulong voiceChannelId, out VoiceClient? voiceClient)
    {
        var removed = _voiceClientDictionary.TryRemove(voiceChannelId, out var dictVoiceClient);
        voiceClient = dictVoiceClient;
        return removed;
    }
}
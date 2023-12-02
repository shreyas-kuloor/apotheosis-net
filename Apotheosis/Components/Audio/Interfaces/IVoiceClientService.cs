using NetCord.Gateway.Voice;

namespace Apotheosis.Components.Audio.Interfaces;

public interface IVoiceClientService
{
    void StoreVoiceClient(ulong voiceChannelId, VoiceClient voiceClient);

    bool TryGetVoiceClient(ulong voiceChannelId, out VoiceClient? voiceClient);

    bool TryRemoveVoiceClient(ulong voiceChannelId, out VoiceClient? voiceClient);
}
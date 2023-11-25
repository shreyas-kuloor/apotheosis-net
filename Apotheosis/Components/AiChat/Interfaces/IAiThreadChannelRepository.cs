using Apotheosis.Components.AiChat.Models;

namespace Apotheosis.Components.AiChat.Interfaces;

public interface IAiThreadChannelRepository
{
    void StoreThreadChannel(ulong threadId);

    void ClearExpiredThreadChannels();

    IEnumerable<ThreadChannelDto> GetStoredThreadChannels();
}
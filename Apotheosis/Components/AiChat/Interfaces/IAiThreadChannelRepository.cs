using Apotheosis.Components.AiChat.Models;

namespace Apotheosis.Components.AiChat.Interfaces;

public interface IAiThreadChannelRepository
{
    void StoreThreadChannel(ThreadChannelDto threadChannelDto);

    void ClearExpiredThreadChannels();

    IEnumerable<ThreadChannelDto> GetStoredActiveThreadChannels();
}
using Apotheosis.Core.Components.AiChat.Models;

namespace Apotheosis.Core.Components.AiChat.Interfaces;

public interface IAiThreadChannelRepository
{
    void StoreThreadChannel(ThreadChannelDto threadChannelDto);

    void ClearExpiredThreadChannels();

    IEnumerable<ThreadChannelDto> GetStoredActiveThreadChannels();
}
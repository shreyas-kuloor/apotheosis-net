using Apotheosis.Core.Features.AiChat.Models;

namespace Apotheosis.Core.Features.AiChat.Interfaces;

public interface IAiThreadChannelRepository
{
    void StoreThreadChannel(ThreadChannelDto threadChannelDto);

    void ClearExpiredThreadChannels();

    IEnumerable<ThreadChannelDto> GetStoredActiveThreadChannels();
}
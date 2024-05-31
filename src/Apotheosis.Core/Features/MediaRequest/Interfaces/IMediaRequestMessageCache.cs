namespace Apotheosis.Core.Features.MediaRequest.Interfaces;
public interface IMediaRequestMessageCache
{
    List<ulong> GetAssociatedMediaRequestMessages(ulong messageId);
    void StoreAssociatedMediaRequestMessages(ulong messageId, ulong associatedMessageId);
    void ClearAssociatedMediaRequestMessages(ulong messageId);
}

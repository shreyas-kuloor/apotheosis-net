using Apotheosis.Core.Features.MediaRequest.Interfaces;
using Apotheosis.Core.Utils;

namespace Apotheosis.Core.Features.MediaRequest.Services;

[Singleton]
public sealed class MediaRequestMessageCache : IMediaRequestMessageCache
{
    readonly List<Tree<ulong>> mediaRequestMessageTrees = [];

    public void ClearAssociatedMediaRequestMessages(ulong messageId)
    {
        var tree = mediaRequestMessageTrees.FirstOrDefault(t => t.Contains(messageId));

        if (tree != null)
        {
            mediaRequestMessageTrees.Remove(tree);
        }
    }

    public List<ulong> GetAssociatedMediaRequestMessages(ulong messageId)
    {
        var tree = mediaRequestMessageTrees.FirstOrDefault(t => t.Contains(messageId));
        return tree?.GetAllNodesInTree() ?? [];
    }

    public void StoreAssociatedMediaRequestMessages(ulong messageId, ulong associatedMessageId)
    {
        var tree = mediaRequestMessageTrees.FirstOrDefault(t => t.Contains(messageId));

        if (tree == null)
        {
            tree = new Tree<ulong>() { Root = new TreeNode<ulong>(messageId) };
            tree.AddNode(messageId, associatedMessageId);
            mediaRequestMessageTrees.Add(tree);
        }
        else
        {
            tree.AddNode(messageId, associatedMessageId);
        }

    }
}

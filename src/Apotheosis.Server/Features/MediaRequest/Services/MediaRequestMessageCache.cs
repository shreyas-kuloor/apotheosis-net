using Apotheosis.Core.Features.MediaRequest.Interfaces;
using Apotheosis.Server.Utils;

namespace Apotheosis.Server.Features.MediaRequest.Services;

[Singleton]
public sealed class MediaRequestMessageCache : IMediaRequestMessageCache
{
    readonly List<Tree<ulong>> _mediaRequestMessageTrees = [];

    public void ClearAssociatedMediaRequestMessages(ulong messageId)
    {
        var tree = _mediaRequestMessageTrees.FirstOrDefault(t => t.Contains(messageId));

        if (tree != null)
        {
            _mediaRequestMessageTrees.Remove(tree);
        }
    }

    public List<ulong> GetAssociatedMediaRequestMessages(ulong messageId)
    {
        var tree = _mediaRequestMessageTrees.FirstOrDefault(t => t.Contains(messageId));
        return tree?.GetAllNodesInTree() ?? [];
    }

    public void StoreAssociatedMediaRequestMessages(ulong messageId, ulong associatedMessageId)
    {
        var tree = _mediaRequestMessageTrees.FirstOrDefault(t => t.Contains(messageId));

        if (tree == null)
        {
            tree = new Tree<ulong> { Root = new TreeNode<ulong>(messageId) };
            tree.AddNode(messageId, associatedMessageId);
            _mediaRequestMessageTrees.Add(tree);
        }
        else
        {
            tree.AddNode(messageId, associatedMessageId);
        }

    }
}

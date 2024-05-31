namespace Apotheosis.Core.Utils;

public sealed class Tree<T> where T : struct
{
    readonly Dictionary<T, TreeNode<T>> nodeLookup = [];

    public TreeNode<T> Root { get; set; } = default!;

    public void AddNode(T parentValue, T childValue)
    {
        var parentNode = GetNode(parentValue);
        if (parentNode == null)
        {
            parentNode = new TreeNode<T>(parentValue);
            nodeLookup[parentValue] = parentNode;
            Root ??= parentNode;
        }

        var childNode = new TreeNode<T>(childValue);
        parentNode.Children.Add(childNode);
        nodeLookup[childValue] = childNode;
    }

    public bool Contains(T value)
    {
        return nodeLookup.ContainsKey(value);
    }

    public TreeNode<T>? GetNode(T value)
    {
        nodeLookup.TryGetValue(value, out var node);
        return node;
    }

    public List<T> GetAllNodesInTree()
    {
        return new List<T>(nodeLookup.Keys);
    }
}

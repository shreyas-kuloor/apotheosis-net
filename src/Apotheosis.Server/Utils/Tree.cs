namespace Apotheosis.Server.Utils;

public sealed class Tree<T> where T : struct
{
    readonly Dictionary<T, TreeNode<T>> _nodeLookup = [];

    public TreeNode<T>? Root { get; set; }

    public void AddNode(T parentValue, T childValue)
    {
        var parentNode = GetNode(parentValue);
        if (parentNode == null)
        {
            parentNode = new TreeNode<T>(parentValue);
            _nodeLookup[parentValue] = parentNode;
            Root ??= parentNode;
        }

        var childNode = new TreeNode<T>(childValue);
        parentNode.Children.Add(childNode);
        _nodeLookup[childValue] = childNode;
    }

    public bool Contains(T value)
    {
        return _nodeLookup.ContainsKey(value);
    }

    TreeNode<T>? GetNode(T value)
    {
        _nodeLookup.TryGetValue(value, out var node);
        return node;
    }

    public List<T> GetAllNodesInTree()
    {
        return [.._nodeLookup.Keys];
    }
}

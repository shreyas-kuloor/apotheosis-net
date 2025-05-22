namespace Apotheosis.Server.Utils;
public class TreeNode<T>(T value)
{
    public T Value { get; set; } = value;
    public List<TreeNode<T>> Children { get; set; } = [];
}

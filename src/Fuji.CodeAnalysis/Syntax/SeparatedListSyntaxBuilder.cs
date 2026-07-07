namespace Fuji.CodeAnalysis.Syntax;

public class SeparatedListSyntaxBuilder<TNode> where TNode : SyntaxNode
{
    public SeparatedListSyntaxBuilder(Func<List<SyntaxNode>, TNode> createNode)
    {
        _createNode = createNode;
    }

    private readonly List<SyntaxNode> _children = new();
    private readonly Func<List<SyntaxNode>, TNode> _createNode;

    public void AddNode(TNode node)
    {
        _children.Add(node);
    }

    public void AddSeparator(SyntaxToken separator)
    {
        _children.Add(separator);
    }

    public TNode Build()
    {
        return _createNode(_children);
    }
}
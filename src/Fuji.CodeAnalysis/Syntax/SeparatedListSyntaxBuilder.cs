namespace Fuji.CodeAnalysis.Syntax;

public class SeparatedListSyntaxBuilder<TNode, TList> where TNode : SyntaxNode where TList : ListSyntax
{
    private readonly List<SyntaxNode> _children = new();
    private readonly Func<SyntaxNode[], TList> _createNode;

    public SeparatedListSyntaxBuilder(Func<SyntaxNode[], TList> createNode)
    {
        _createNode = createNode;
    }

    public void AddNode(TNode node)
    {
        _children.Add(node);
    }

    public void AddSeparator(SyntaxToken separator)
    {
        _children.Add(separator);
    }

    public TList Build()
    {
        return _createNode(_children.ToArray());
    }
}
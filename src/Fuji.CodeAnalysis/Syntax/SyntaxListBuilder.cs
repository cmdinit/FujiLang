namespace Fuji.CodeAnalysis.Syntax;

public class SyntaxListBuilder<TNode, TList> where TNode : SyntaxNode where TList : ListSyntax
{
    private readonly List<TNode> _nodes = [];
    private readonly Func<TNode[], TList> _create;

    public SyntaxListBuilder(Func<TNode[], TList> create)
    {
        _create = create;
    }

    public void Add(TNode node)
    {
        _nodes.Add(node);
    }

    public TList Build()
    {
        return _create(_nodes.ToArray());
    }
}
namespace Fuji.CodeAnalysis.Syntax;

public class SyntaxListBuilder<TNode> where TNode : SyntaxNode
{
    private readonly List<SyntaxNode> _nodes = [];
    private readonly Func<List<SyntaxNode>, TNode> _create;

    public SyntaxListBuilder(Func<List<SyntaxNode>, TNode> create)
    {
        _create = create;
    }

    public void Add(TNode node)
    {
        _nodes.Add(node);
    }

    public TNode Build()
    {
        return _create(_nodes);
    }
}
namespace Fuji.CodeAnalysis.Syntax;

public abstract class ListSyntax : SyntaxNode
{
    protected List<SyntaxNode> _children = new();

    protected ListSyntax(SyntaxKind kind, List<SyntaxNode> children) : base(kind)
    {
        _children = children;
        SlotCount = _children.Count;
        foreach (var child in _children)
        {
            AdjustWidth(child);
        }
    }

    public override SyntaxNode? GetSlot(int index)
    {
        if (index < 0 || index >= _children.Count)
            return null;

        return _children[index];
    }
}

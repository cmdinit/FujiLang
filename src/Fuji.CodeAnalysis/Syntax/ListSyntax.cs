namespace Fuji.CodeAnalysis.Syntax;

public abstract class ListSyntax : SyntaxNode
{
    protected SyntaxNode[] _children = Array.Empty<SyntaxNode>();

    protected ListSyntax(SyntaxKind kind, SyntaxNode[] children) : base(kind)
    {
        _children = children;
        SlotCount = _children.Length;
        foreach (var child in _children)
        {
            AdjustWidth(child);
        }
    }

    public override SyntaxNode? GetSlot(int index)
    {
        if (index < 0 || index >= _children.Length)
            return null;

        return _children[index];
    }

    public override bool IsList => true;
}

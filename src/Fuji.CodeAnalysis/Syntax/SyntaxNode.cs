namespace Fuji.CodeAnalysis.Syntax;

public abstract class SyntaxNode
{
    public SyntaxKind Kind { get; }
    public int FullWidth { get; private set; }
    public int SlotCount { get; protected set; }
    protected SyntaxNode(SyntaxKind kind)
    {
        Kind = kind;
    }

    protected SyntaxNode(SyntaxKind kind, int fullWidth)
    {
        Kind = kind;
        FullWidth = fullWidth;
    }

    protected void AdjustWidth(SyntaxNode node)
    {
        FullWidth += node.FullWidth;
    }

    public abstract SyntaxNode? GetSlot(int index);

    public SyntaxNode? GetFirstTerminal()
    {
        SyntaxNode? node = this;
        do
        {
            SyntaxNode? firstChild = null;
            for (int i = 0, n = node.SlotCount; i < n; i++)
            {
                var child = node.GetSlot(i);
                if (child != null)
                {
                    firstChild = child;
                    break;
                }
            }
            node = firstChild;
        }
        while (node?.SlotCount > 0);

        return node;
    }

    public SyntaxNode? GetLastTerminal()
    {
        SyntaxNode? node = this;
        do
        {
            SyntaxNode? lastChild = null;
            for (int i = node.SlotCount - 1; i >= 0; i--)
            {
                var child = node.GetSlot(i);
                if (child != null)
                {
                    lastChild = child;
                    break;
                }
            }
            node = lastChild;
        }
        while (node?.SlotCount > 0);

        return node;
    }

    public int GetLeadingTriviaWidth()
    {
        if (FullWidth == 0)
            return 0;

        if (this is SyntaxToken token)
            return token.LeadingTriviaLength;

        var firstTerminal = GetFirstTerminal();
        if (firstTerminal is SyntaxToken t)
            return t.LeadingTriviaLength;

        return 0;
    }

    public virtual bool IsList => false;
}

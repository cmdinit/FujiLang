namespace Fuji.CodeAnalysis.Syntax;

public class SyntaxToken : SyntaxNode
{
    public SyntaxToken(SyntaxKind kind, string text, int start, int leadingTriviaLength)
        : base(kind, text.Length + leadingTriviaLength)
    {
        Text = text;
        Start = start;
        LeadingTriviaLength = leadingTriviaLength;
    }

    public string Text { get; }
    public int Start { get; }
    public int LeadingTriviaLength { get; }
    public override SyntaxNode? GetSlot(int index)
    {
        throw new Exception("Unreachable");
    }
}

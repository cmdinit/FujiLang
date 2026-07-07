namespace Fuji.CodeAnalysis.Syntax;

public abstract class ExpressionSyntax : SyntaxNode
{
    protected ExpressionSyntax(SyntaxKind kind) : base(kind)
    {
    }
}

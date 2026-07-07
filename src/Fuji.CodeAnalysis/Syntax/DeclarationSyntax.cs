namespace Fuji.CodeAnalysis.Syntax;

public abstract class DeclarationSyntax : SyntaxNode
{
    protected DeclarationSyntax(SyntaxKind kind) : base(kind)
    {
    }
}

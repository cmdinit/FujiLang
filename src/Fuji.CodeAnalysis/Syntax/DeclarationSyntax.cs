namespace Fuji.CodeAnalysis.Syntax;

public abstract class DeclarationSyntax : SyntaxNode
{
    protected DeclarationSyntax(SyntaxKind kind) : base(kind)
    {
    }
}

public abstract class MemberDeclarationSyntax : DeclarationSyntax
{
    protected MemberDeclarationSyntax(SyntaxKind kind) : base(kind)
    {
    }
}

public abstract class RootDeclarationSyntax : DeclarationSyntax
{
    protected RootDeclarationSyntax(SyntaxKind kind) : base(kind)
    {
    }
}
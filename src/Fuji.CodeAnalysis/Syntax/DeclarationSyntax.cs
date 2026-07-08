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

public class CompilationUnitSyntax : SyntaxNode
{
    public static CompilationUnitSyntax Create(RootDeclarationListSyntax declarations, SyntaxToken eofToken)
    {
        return new CompilationUnitSyntax(SyntaxKind.CompilationUnit, declarations, eofToken);
    }

    private CompilationUnitSyntax(SyntaxKind kind, RootDeclarationListSyntax declarations, SyntaxToken eofToken)
        : base(kind)
    {
        SlotCount = 2;
        Declarations = declarations;
        EofToken = eofToken;
        AdjustWidth(Declarations);
        AdjustWidth(EofToken);
    }

    public RootDeclarationListSyntax Declarations { get; }
    public SyntaxToken EofToken { get; }

    public override SyntaxNode? GetSlot(int index)
    {
        return index switch
        {
            0 => Declarations,
            1 => EofToken,
            _ => null
        };
    }
}

public class StructDeclarationSyntax : RootDeclarationSyntax
{
    public static StructDeclarationSyntax Create(SyntaxToken structKeyword, SyntaxToken identifier, SyntaxToken openBraceToken, MemberDeclarationListSyntax declarations, SyntaxToken closeBraceToken)
    {
        return new StructDeclarationSyntax(SyntaxKind.StructDeclaration, structKeyword, identifier, openBraceToken, declarations, closeBraceToken);
    }

    private StructDeclarationSyntax(SyntaxKind kind, SyntaxToken structKeyword, SyntaxToken identifier, SyntaxToken openBraceToken, MemberDeclarationListSyntax declarations, SyntaxToken closeBraceToken)
        : base(kind)
    {
        SlotCount = 5;
        StructKeyword = structKeyword;
        Identifier = identifier;
        OpenBraceToken = openBraceToken;
        Declarations = declarations;
        CloseBraceToken = closeBraceToken;
        AdjustWidth(StructKeyword);
        AdjustWidth(Identifier);
        AdjustWidth(OpenBraceToken);
        AdjustWidth(Declarations);
        AdjustWidth(CloseBraceToken);
    }

    public SyntaxToken StructKeyword { get; }
    public SyntaxToken Identifier { get; }
    public SyntaxToken OpenBraceToken { get; }
    public MemberDeclarationListSyntax Declarations { get; }
    public SyntaxToken CloseBraceToken { get; }

    public override SyntaxNode? GetSlot(int index)
    {
        return index switch
        {
            0 => StructKeyword,
            1 => Identifier,
            2 => OpenBraceToken,
            3 => Declarations,
            4 => CloseBraceToken,
            _ => null
        };
    }
}

public class FieldDeclarationSyntax : MemberDeclarationSyntax
{
    public static FieldDeclarationSyntax Create(SyntaxToken identifier, SyntaxToken colonToken, TypeSyntax type, SyntaxToken semicolonToken)
    {
        return new FieldDeclarationSyntax(SyntaxKind.FieldDeclaration, identifier, colonToken, type, semicolonToken);
    }

    private FieldDeclarationSyntax(SyntaxKind kind, SyntaxToken identifier, SyntaxToken colonToken, TypeSyntax type, SyntaxToken semicolonToken)
        : base(kind)
    {
        SlotCount = 4;
        Identifier = identifier;
        ColonToken = colonToken;
        Type = type;
        SemicolonToken = semicolonToken;
        AdjustWidth(Identifier);
        AdjustWidth(ColonToken);
        AdjustWidth(Type);
        AdjustWidth(SemicolonToken);
    }

    public SyntaxToken Identifier { get; }
    public SyntaxToken ColonToken { get; }
    public TypeSyntax Type { get; }
    public SyntaxToken SemicolonToken { get; }

    public override SyntaxNode? GetSlot(int index)
    {
        return index switch
        {
            0 => Identifier,
            1 => ColonToken,
            2 => Type,
            3 => SemicolonToken,
            _ => null
        };
    }
}

public class FunctionDeclarationSyntax : RootDeclarationSyntax
{
    public static FunctionDeclarationSyntax Create(SyntaxToken funcKeyword, SyntaxToken identifier, SyntaxToken openParenToken, ParameterListSyntax parameters, SyntaxToken closeParenToken, BlockSyntax body)
    {
        return new FunctionDeclarationSyntax(SyntaxKind.FunctionDeclaration, funcKeyword, identifier, openParenToken, parameters, closeParenToken, body);
    }

    private FunctionDeclarationSyntax(SyntaxKind kind, SyntaxToken funcKeyword, SyntaxToken identifier, SyntaxToken openParenToken, ParameterListSyntax parameters, SyntaxToken closeParenToken, BlockSyntax body)
        : base(kind)
    {
        SlotCount = 6;
        FuncKeyword = funcKeyword;
        Identifier = identifier;
        OpenParenToken = openParenToken;
        Parameters = parameters;
        CloseParenToken = closeParenToken;
        Body = body;
        AdjustWidth(FuncKeyword);
        AdjustWidth(Identifier);
        AdjustWidth(OpenParenToken);
        AdjustWidth(Parameters);
        AdjustWidth(CloseParenToken);
        AdjustWidth(Body);
    }

    public SyntaxToken FuncKeyword { get; }
    public SyntaxToken Identifier { get; }
    public SyntaxToken OpenParenToken { get; }
    public ParameterListSyntax Parameters { get; }
    public SyntaxToken CloseParenToken { get; }
    public BlockSyntax Body { get; }

    public override SyntaxNode? GetSlot(int index)
    {
        return index switch
        {
            0 => FuncKeyword,
            1 => Identifier,
            2 => OpenParenToken,
            3 => Parameters,
            4 => CloseParenToken,
            5 => Body,
            _ => null
        };
    }
}

public class ParameterSyntax : SyntaxNode
{
    public static ParameterSyntax Create(SyntaxToken identifier, SyntaxToken colonToken, TypeSyntax type)
    {
        return new ParameterSyntax(SyntaxKind.Parameter, identifier, colonToken, type);
    }

    private ParameterSyntax(SyntaxKind kind, SyntaxToken identifier, SyntaxToken colonToken, TypeSyntax type)
        : base(kind)
    {
        Identifier = identifier;
        ColonToken = colonToken;
        Type = type;
        AdjustWidth(Identifier);
        AdjustWidth(ColonToken);
        AdjustWidth(Type);
    }

    public SyntaxToken Identifier { get; }
    public SyntaxToken ColonToken { get; }
    public TypeSyntax Type { get; }

    public override SyntaxNode? GetSlot(int index)
    {
        return index switch
        {
            0 => Identifier,
            1 => ColonToken,
            2 => Type,
            _ => null
        };
    }
}

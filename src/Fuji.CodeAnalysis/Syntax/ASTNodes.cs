namespace Fuji.CodeAnalysis.Syntax;


public class PrefixUnaryExpressionSyntax : ExpressionSyntax
{
    public static PrefixUnaryExpressionSyntax Create(SyntaxToken operatorToken, ExpressionSyntax operand)
    {
        var kind = operatorToken.Kind.GetPrefixUnaryOperatorKind();
        if (kind == null)
        {
            throw new Exception($"Invalid prefix unary operator: {operatorToken.Kind}");
        }

        return new PrefixUnaryExpressionSyntax(kind.Value, operatorToken, operand);
    }

    private PrefixUnaryExpressionSyntax(SyntaxKind kind, SyntaxToken operatorToken, ExpressionSyntax operand)
        : base(kind)
    {
        SlotCount = 2;
        OperatorToken = operatorToken;
        Operand = operand;
        AdjustWidth(OperatorToken);
        AdjustWidth(Operand);
    }

    public SyntaxToken OperatorToken { get; }
    public ExpressionSyntax Operand { get; }

    public override SyntaxNode? GetSlot(int index)
    {
        return index switch
        {
            0 => OperatorToken,
            1 => Operand,
            _ => null
        };
    }
}

public class BinaryExpressionSyntax : ExpressionSyntax
{
    public static BinaryExpressionSyntax Create(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
    {
        var kind = operatorToken.Kind.GetBinaryOperatorKind();
        if (kind == null)
        {
            throw new Exception($"Invalid binary operator: {operatorToken.Kind}");
        }

        return new BinaryExpressionSyntax(kind.Value, left, operatorToken, right);
    }

    private BinaryExpressionSyntax(SyntaxKind kind, ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
        : base(kind)
    {
        SlotCount = 3;
        Left = left;
        OperatorToken = operatorToken;
        Right = right;
        AdjustWidth(Left);
        AdjustWidth(OperatorToken);
        AdjustWidth(Right);
    }

    public ExpressionSyntax Left { get; }
    public SyntaxToken OperatorToken { get; }
    public ExpressionSyntax Right { get; }

    public override SyntaxNode? GetSlot(int index)
    {
        return index switch
        {
            0 => Left,
            1 => OperatorToken,
            2 => Right,
            _ => null
        };
    }
}

public class ParenthesisExpressionSyntax : ExpressionSyntax
{
    public ParenthesisExpressionSyntax(SyntaxToken openParenToken, ExpressionSyntax expression, SyntaxToken closeParenToken)
        : base(SyntaxKind.ParenthesizedExpression)
    {
        SlotCount = 3;
        OpenParenToken = openParenToken;
        Expression = expression;
        CloseParenToken = closeParenToken;
        AdjustWidth(OpenParenToken);
        AdjustWidth(Expression);
        AdjustWidth(CloseParenToken);
    }

    public SyntaxToken OpenParenToken { get; }
    public ExpressionSyntax Expression { get; }
    public SyntaxToken CloseParenToken { get; }

    public override SyntaxNode? GetSlot(int index)
    {
        return index switch
        {
            0 => OpenParenToken,
            1 => Expression,
            2 => CloseParenToken,
            _ => null
        };
    }
}

public class MemberAccessExpressionSyntax : ExpressionSyntax
{
    public MemberAccessExpressionSyntax(ExpressionSyntax expression, SyntaxToken dotToken, SyntaxToken identifierToken)
        : base(SyntaxKind.MemberAccessExpression)
    {
        SlotCount = 3;
        Expression = expression;
        DotToken = dotToken;
        IdentifierToken = identifierToken;
        AdjustWidth(Expression);
        AdjustWidth(DotToken);
        AdjustWidth(IdentifierToken);
    }

    public ExpressionSyntax Expression { get; }
    public SyntaxToken DotToken { get; }
    public SyntaxToken IdentifierToken { get; }

    public override SyntaxNode? GetSlot(int index)
    {
        return index switch
        {
            0 => Expression,
            1 => DotToken,
            2 => IdentifierToken,
            _ => null
        };
    }
}

public class LiteralExpressionSyntax : ExpressionSyntax
{
    public static LiteralExpressionSyntax Create(SyntaxToken literalToken)
    {
        var kind = literalToken.Kind.GetLiteralExpressionKind();
        if (kind == null)
        {
            throw new Exception($"Invalid literal token: {literalToken.Kind}");
        }

        return new LiteralExpressionSyntax(kind.Value, literalToken);
    }

    private LiteralExpressionSyntax(SyntaxKind kind, SyntaxToken literalToken)
        : base(kind)
    {
        SlotCount = 1;
        LiteralToken = literalToken;
        AdjustWidth(LiteralToken);
    }

    public SyntaxToken LiteralToken { get; }

    public override SyntaxNode? GetSlot(int index)
    {
        return index switch
        {
            0 => LiteralToken,
            _ => null
        };
    }
}

public class InvocationExpressionSyntax : ExpressionSyntax
{
    public InvocationExpressionSyntax(ExpressionSyntax expression, SyntaxToken openParenToken, ArgumentListSyntax arguments, SyntaxToken closeParenToken)
        : base(SyntaxKind.InvocationExpression)
    {
        SlotCount = 4;
        Expression = expression;
        OpenParenToken = openParenToken;
        Arguments = arguments;
        CloseParenToken = closeParenToken;
        AdjustWidth(Expression);
        AdjustWidth(OpenParenToken);
        AdjustWidth(Arguments);
        AdjustWidth(CloseParenToken);
    }

    public ExpressionSyntax Expression { get; }
    public SyntaxToken OpenParenToken { get; }
    public ArgumentListSyntax Arguments { get; }
    public SyntaxToken CloseParenToken { get; }

    public override SyntaxNode? GetSlot(int index)
    {
        return index switch
        {
            0 => Expression,
            1 => OpenParenToken,
            2 => Arguments,
            3 => CloseParenToken,
            _ => null
        };
    }
}


public class ArgumentSyntax : SyntaxNode
{
    public static ArgumentSyntax Create(SyntaxToken identifier, SyntaxToken colonToken, ExpressionSyntax expression)
    {
        return new ArgumentSyntax(SyntaxKind.Argument, identifier, colonToken, expression);
    }

    private ArgumentSyntax(SyntaxKind kind, SyntaxToken identifier, SyntaxToken colonToken, ExpressionSyntax expression)
        : base(kind)
    {
        SlotCount = 3;
        Identifier = identifier;
        ColonToken = colonToken;
        Expression = expression;
        AdjustWidth(Identifier);
        AdjustWidth(ColonToken);
        AdjustWidth(Expression);
    }

    public SyntaxToken Identifier { get; }
    public SyntaxToken ColonToken { get; }
    public ExpressionSyntax Expression { get; }

    public override SyntaxNode? GetSlot(int index)
    {
        return index switch
        {
            0 => Identifier,
            1 => ColonToken,
            2 => Expression,
            _ => null
        };
    }
}

public class BlockSyntax : StatementSyntax
{
    public static BlockSyntax Create(SyntaxToken openBraceToken, StatementListSyntax statements, SyntaxToken closeBraceToken)
    {
        return new BlockSyntax(SyntaxKind.Block, openBraceToken, statements, closeBraceToken);
    }

    private BlockSyntax(SyntaxKind kind, SyntaxToken openBraceToken, StatementListSyntax statements, SyntaxToken closeBraceToken)
        : base(kind)
    {
        SlotCount = 3;
        OpenBraceToken = openBraceToken;
        Statements = statements;
        CloseBraceToken = closeBraceToken;
        AdjustWidth(OpenBraceToken);
        AdjustWidth(Statements);
        AdjustWidth(CloseBraceToken);
    }

    public SyntaxToken OpenBraceToken { get; }
    public StatementListSyntax Statements { get; }
    public SyntaxToken CloseBraceToken { get; }

    public override SyntaxNode? GetSlot(int index)
    {
        return index switch
        {
            0 => OpenBraceToken,
            1 => Statements,
            2 => CloseBraceToken,
            _ => null
        };
    }
}

public class ExpressionStatementSyntax : StatementSyntax
{
    public static ExpressionStatementSyntax Create(ExpressionSyntax expression, SyntaxToken semicolonToken)
    {
        return new ExpressionStatementSyntax(SyntaxKind.ExpressionStatement, expression, semicolonToken);
    }

    private ExpressionStatementSyntax(SyntaxKind kind, ExpressionSyntax expression, SyntaxToken semicolonToken)
        : base(kind)
    {
        SlotCount = 2;
        Expression = expression;
        SemicolonToken = semicolonToken;
        AdjustWidth(Expression);
        AdjustWidth(SemicolonToken);
    }

    public ExpressionSyntax Expression { get; }
    public SyntaxToken SemicolonToken { get; }

    public override SyntaxNode? GetSlot(int index)
    {
        return index switch
        {
            0 => Expression,
            1 => SemicolonToken,
            _ => null
        };
    }
}

public class ReturnStatementSyntax : StatementSyntax
{
    public static ReturnStatementSyntax Create(SyntaxToken returnKeyword, ExpressionSyntax? expression, SyntaxToken semicolonToken)
    {
        return new ReturnStatementSyntax(SyntaxKind.ReturnStatement, returnKeyword, expression, semicolonToken);
    }

    private ReturnStatementSyntax(SyntaxKind kind, SyntaxToken returnKeyword, ExpressionSyntax? expression, SyntaxToken semicolonToken)
        : base(kind)
    {
        SlotCount = 3;
        ReturnKeyword = returnKeyword;
        Expression = expression;
        SemicolonToken = semicolonToken;
        AdjustWidth(ReturnKeyword);
        if (Expression != null)
            AdjustWidth(Expression);
        AdjustWidth(SemicolonToken);
    }

    public SyntaxToken ReturnKeyword { get; }
    public ExpressionSyntax? Expression { get; }
    public SyntaxToken SemicolonToken { get; }

    public override SyntaxNode? GetSlot(int index)
    {
        return index switch
        {
            0 => ReturnKeyword,
            1 => Expression,
            2 => SemicolonToken,
            _ => null
        };
    }
}

public class EmptyStatementSyntax : StatementSyntax
{
    public static EmptyStatementSyntax Create(SyntaxToken semicolonToken)
    {
        return new EmptyStatementSyntax(SyntaxKind.EmptyStatement, semicolonToken);
    }

    private EmptyStatementSyntax(SyntaxKind kind, SyntaxToken semicolonToken)
        : base(kind)
    {
        SlotCount = 1;
        SemicolonToken = semicolonToken;
        AdjustWidth(SemicolonToken);
    }

    public SyntaxToken SemicolonToken { get; }

    public override SyntaxNode? GetSlot(int index)
    {
        return index switch
        {
            0 => SemicolonToken,
            _ => null
        };
    }
}

public class ContinueStatementSyntax : StatementSyntax
{
    public static ContinueStatementSyntax Create(SyntaxToken continueKeyword, SyntaxToken semicolonToken)
    {
        return new ContinueStatementSyntax(SyntaxKind.ContinueStatement, continueKeyword, semicolonToken);
    }

    private ContinueStatementSyntax(SyntaxKind kind, SyntaxToken continueKeyword, SyntaxToken semicolonToken)
        : base(kind)
    {
        SlotCount = 2;
        ContinueKeyword = continueKeyword;
        SemicolonToken = semicolonToken;
        AdjustWidth(ContinueKeyword);
        AdjustWidth(SemicolonToken);
    }

    public SyntaxToken ContinueKeyword { get; }
    public SyntaxToken SemicolonToken { get; }

    public override SyntaxNode? GetSlot(int index)
    {
        return index switch
        {
            0 => ContinueKeyword,
            1 => SemicolonToken,
            _ => null
        };
    }
}

public class BreakStatementSyntax : StatementSyntax
{
    public static BreakStatementSyntax Create(SyntaxToken breakKeyword, SyntaxToken semicolonToken)
    {
        return new BreakStatementSyntax(SyntaxKind.BreakStatement, breakKeyword, semicolonToken);
    }

    private BreakStatementSyntax(SyntaxKind kind, SyntaxToken breakKeyword, SyntaxToken semicolonToken)
        : base(kind)
    {
        SlotCount = 2;
        BreakKeyword = breakKeyword;
        SemicolonToken = semicolonToken;
        AdjustWidth(BreakKeyword);
        AdjustWidth(SemicolonToken);
    }

    public SyntaxToken BreakKeyword { get; }
    public SyntaxToken SemicolonToken { get; }

    public override SyntaxNode? GetSlot(int index)
    {
        return index switch
        {
            0 => BreakKeyword,
            1 => SemicolonToken,
            _ => null
        };
    }
}

public class ElseClauseSyntax : SyntaxNode
{
    public static ElseClauseSyntax Create(SyntaxToken elseKeyword, StatementSyntax statement)
    {
        return new ElseClauseSyntax(SyntaxKind.ElseClause, elseKeyword, statement);
    }

    private ElseClauseSyntax(SyntaxKind kind, SyntaxToken elseKeyword, StatementSyntax statement)
        : base(kind)
    {
        SlotCount = 2;
        ElseKeyword = elseKeyword;
        Statement = statement;
        AdjustWidth(ElseKeyword);
        AdjustWidth(Statement);
    }

    public SyntaxToken ElseKeyword { get; }
    public StatementSyntax Statement { get; }

    public override SyntaxNode? GetSlot(int index)
    {
        return index switch
        {
            0 => ElseKeyword,
            1 => Statement,
            _ => null
        };
    }
}

public class IfStatementSyntax : StatementSyntax
{
    public static IfStatementSyntax Create(SyntaxToken ifKeyword, SyntaxToken openParenToken, ExpressionSyntax condition, SyntaxToken closeParenToken, StatementSyntax body, ElseClauseSyntax? elseClause)
    {
        return new IfStatementSyntax(SyntaxKind.IfStatement, ifKeyword, openParenToken, condition, closeParenToken, body, elseClause);
    }

    private IfStatementSyntax(SyntaxKind kind, SyntaxToken ifKeyword, SyntaxToken openParenToken, ExpressionSyntax condition, SyntaxToken closeParenToken, StatementSyntax body, ElseClauseSyntax? elseClause)
        : base(kind)
    {
        SlotCount = 6;
        IfKeyword = ifKeyword;
        OpenParenToken = openParenToken;
        Condition = condition;
        CloseParenToken = closeParenToken;
        Body = body;
        ElseClause = elseClause;
        AdjustWidth(IfKeyword);
        AdjustWidth(OpenParenToken);
        AdjustWidth(Condition);
        AdjustWidth(CloseParenToken);
        AdjustWidth(Body);
        if (ElseClause != null)
            AdjustWidth(ElseClause);
    }

    public SyntaxToken IfKeyword { get; }
    public SyntaxToken OpenParenToken { get; }
    public ExpressionSyntax Condition { get; }
    public SyntaxToken CloseParenToken { get; }
    public StatementSyntax Body { get; }
    public ElseClauseSyntax? ElseClause { get; }

    public override SyntaxNode? GetSlot(int index)
    {
        return index switch
        {
            0 => IfKeyword,
            1 => OpenParenToken,
            2 => Condition,
            3 => CloseParenToken,
            4 => Body,
            5 => ElseClause,
            _ => null
        };
    }
}

public class LoopStatementSyntax : StatementSyntax
{
    public static LoopStatementSyntax Create(SyntaxToken loopKeyword, StatementSyntax body)
    {
        return new LoopStatementSyntax(SyntaxKind.LoopStatement, loopKeyword, body);
    }

    private LoopStatementSyntax(SyntaxKind kind, SyntaxToken loopKeyword, StatementSyntax body)
        : base(kind)
    {
        SlotCount = 2;
        LoopKeyword = loopKeyword;
        Body = body;
        AdjustWidth(LoopKeyword);
        AdjustWidth(Body);
    }

    public SyntaxToken LoopKeyword { get; }
    public StatementSyntax Body { get; }

    public override SyntaxNode? GetSlot(int index)
    {
        return index switch
        {
            0 => LoopKeyword,
            1 => Body,
            _ => null
        };
    }
}

public class ForeachStatementSyntax : StatementSyntax
{
    public static ForeachStatementSyntax Create(
        SyntaxToken foreachKeyword,
        SyntaxToken openParenToken,
        SyntaxToken identifierToken,
        SyntaxToken colonToken,
        ExpressionSyntax expression,
        SyntaxToken closeParenToken,
        StatementSyntax body)
    {
        return new ForeachStatementSyntax(
            SyntaxKind.ForeachStatement,
            foreachKeyword,
            openParenToken,
            identifierToken,
            colonToken,
            expression,
            closeParenToken,
            body);
    }

    private ForeachStatementSyntax(
        SyntaxKind kind,
        SyntaxToken foreachKeyword,
        SyntaxToken openParenToken,
        SyntaxToken identifierToken,
        SyntaxToken colonToken,
        ExpressionSyntax expression,
        SyntaxToken closeParenToken,
        StatementSyntax body)
        : base(kind)
    {
        SlotCount = 7;
        ForeachKeyword = foreachKeyword;
        OpenParenToken = openParenToken;
        IdentifierToken = identifierToken;
        ColonToken = colonToken;
        Expression = expression;
        CloseParenToken = closeParenToken;
        Body = body;
        AdjustWidth(ForeachKeyword);
        AdjustWidth(OpenParenToken);
        AdjustWidth(IdentifierToken);
        AdjustWidth(ColonToken);
        AdjustWidth(Expression);
        AdjustWidth(CloseParenToken);
        AdjustWidth(Body);
    }

    public SyntaxToken ForeachKeyword { get; }
    public SyntaxToken OpenParenToken { get; }
    public SyntaxToken IdentifierToken { get; }
    public SyntaxToken ColonToken { get; }
    public ExpressionSyntax Expression { get; }
    public SyntaxToken CloseParenToken { get; }
    public StatementSyntax Body { get; }

    public override SyntaxNode? GetSlot(int index)
    {
        return index switch
        {
            0 => ForeachKeyword,
            1 => OpenParenToken,
            2 => IdentifierToken,
            3 => ColonToken,
            4 => Expression,
            5 => CloseParenToken,
            6 => Body,
            _ => null
        };
    }
}

public abstract class TypeSyntax : ExpressionSyntax
{
    protected TypeSyntax(SyntaxKind kind) : base(kind)
    {
    }
}

public class PredefinedTypeSyntax : TypeSyntax
{
    public static PredefinedTypeSyntax Create(SyntaxToken keyword)
    {
        return new PredefinedTypeSyntax(SyntaxKind.PredefinedType, keyword);
    }

    private PredefinedTypeSyntax(SyntaxKind kind, SyntaxToken keyword)
        : base(kind)
    {
        SlotCount = 1;
        Keyword = keyword;
        AdjustWidth(Keyword);
    }

    public SyntaxToken Keyword { get; }

    public override SyntaxNode? GetSlot(int index)
    {
        return index switch
        {
            0 => Keyword,
            _ => null
        };
    }
}

public class IdentifierNameSyntax : TypeSyntax
{
    public static IdentifierNameSyntax Create(SyntaxToken identifier)
    {
        return new IdentifierNameSyntax(SyntaxKind.IdentifierName, identifier);
    }

    private IdentifierNameSyntax(SyntaxKind kind, SyntaxToken identifier)
        : base(kind)
    {
        SlotCount = 1;
        Identifier = identifier;
        AdjustWidth(Identifier);
    }

    public SyntaxToken Identifier { get; }

    public override SyntaxNode? GetSlot(int index)
    {
        return index switch
        {
            0 => Identifier,
            _ => null
        };
    }
}

public class QualifiedNameSyntax : TypeSyntax
{
    public static QualifiedNameSyntax Create(TypeSyntax left, SyntaxToken dotToken, SyntaxToken right)
    {
        return new QualifiedNameSyntax(SyntaxKind.QualifiedName, left, dotToken, right);
    }

    private QualifiedNameSyntax(SyntaxKind kind, TypeSyntax left, SyntaxToken dotToken, SyntaxToken right)
        : base(kind)
    {
        SlotCount = 3;
        Left = left;
        DotToken = dotToken;
        Right = right;
        AdjustWidth(Left);
        AdjustWidth(DotToken);
        AdjustWidth(Right);
    }

    public TypeSyntax Left { get; }
    public SyntaxToken DotToken { get; }
    public SyntaxToken Right { get; }

    public override SyntaxNode? GetSlot(int index)
    {
        return index switch
        {
            0 => Left,
            1 => DotToken,
            2 => Right,
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

public class ArgumentListSyntax : ListSyntax
{
    public static SeparatedListSyntaxBuilder<ArgumentSyntax, ArgumentListSyntax> GetBuilder()
    {
        return new SeparatedListSyntaxBuilder<ArgumentSyntax, ArgumentListSyntax>(Create);
    }

    private static ArgumentListSyntax Create(SyntaxNode[] arguments)
    {
        return new ArgumentListSyntax(arguments);
    }

    private ArgumentListSyntax(SyntaxNode[] arguments)
        : base(SyntaxKind.ArgumentList, arguments)
    {
    }
}

public class ParameterListSyntax : ListSyntax
{
    public static SeparatedListSyntaxBuilder<ParameterSyntax, ParameterListSyntax> GetBuilder()
    {
        return new SeparatedListSyntaxBuilder<ParameterSyntax, ParameterListSyntax>(Create);
    }

    private static ParameterListSyntax Create(SyntaxNode[] parameters)
    {
        return new ParameterListSyntax(parameters);
    }

    private ParameterListSyntax(SyntaxNode[] parameters)
        : base(SyntaxKind.ParameterList, parameters)
    {
    }
}


public class StatementListSyntax : ListSyntax
{
    public static SyntaxListBuilder<StatementSyntax, StatementListSyntax> GetBuilder()
    {
        return new SyntaxListBuilder<StatementSyntax, StatementListSyntax>(Create);
    }

    private StatementListSyntax(StatementSyntax[] statements)
        : base(SyntaxKind.StatementList, statements)
    {
    }
    private static StatementListSyntax Create(StatementSyntax[] statements)
    {
        return new StatementListSyntax(statements);
    }
}

public class RootDeclarationListSyntax : ListSyntax
{
    public static SyntaxListBuilder<RootDeclarationSyntax, RootDeclarationListSyntax> GetBuilder()
    {
        return new SyntaxListBuilder<RootDeclarationSyntax, RootDeclarationListSyntax>(Create);
    }

    private RootDeclarationListSyntax(RootDeclarationSyntax[] declarations)
        : base(SyntaxKind.RootDeclarationList, declarations)
    {
    }
    private static RootDeclarationListSyntax Create(RootDeclarationSyntax[] declarations)
    {
        return new RootDeclarationListSyntax(declarations);
    }
}

public class MemberDeclarationListSyntax : ListSyntax
{
    public static SyntaxListBuilder<MemberDeclarationSyntax, MemberDeclarationListSyntax> GetBuilder()
    {
        return new SyntaxListBuilder<MemberDeclarationSyntax, MemberDeclarationListSyntax>(Create);
    }

    private MemberDeclarationListSyntax(MemberDeclarationSyntax[] declarations)
        : base(SyntaxKind.MemberDeclarationList, declarations)
    {
    }
    private static MemberDeclarationListSyntax Create(MemberDeclarationSyntax[] declarations)
    {
        return new MemberDeclarationListSyntax(declarations);
    }
}

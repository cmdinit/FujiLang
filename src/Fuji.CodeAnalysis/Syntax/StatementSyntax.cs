namespace Fuji.CodeAnalysis.Syntax;

public abstract class StatementSyntax : SyntaxNode
{
    protected StatementSyntax(SyntaxKind kind) : base(kind)
    {
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

namespace Fuji.CodeAnalysis.Syntax;

public abstract class ExpressionSyntax : SyntaxNode
{
    protected ExpressionSyntax(SyntaxKind kind) : base(kind)
    {
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

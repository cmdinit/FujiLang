namespace Fuji.CodeAnalysis.Syntax;

public static class SyntaxFacts
{
    public static SyntaxKind GetKeywordOrIdentifierKind(string text)
    {
        return text switch
        {
            "if" => SyntaxKind.IfKeyword,
            "else" => SyntaxKind.ElseKeyword,
            "for" => SyntaxKind.ForKeyword,
            "return" => SyntaxKind.ReturnKeyword,
            _ => SyntaxKind.IdentifierToken
        };
    }

    public static SyntaxKind? GetPrefixUnaryOperatorKind(this SyntaxKind kind)
    {
        return kind switch
        {
            SyntaxKind.PlusToken => SyntaxKind.UnaryPlusExpression,
            SyntaxKind.MinusToken => SyntaxKind.UnaryMinusExpression,
            SyntaxKind.BangToken => SyntaxKind.LogicalNotExpression,
            SyntaxKind.TildeToken => SyntaxKind.BitwiseNotExpression,
            _ => null
        };
    }

    public static SyntaxKind? GetBinaryOperatorKind(this SyntaxKind kind)
    {
        return kind switch
        {
            SyntaxKind.PlusToken => SyntaxKind.AddExpression,
            SyntaxKind.MinusToken => SyntaxKind.SubtractExpression,
            SyntaxKind.StarToken => SyntaxKind.MultiplyExpression,
            SyntaxKind.SlashToken => SyntaxKind.DivideExpression,
            SyntaxKind.EqEqToken => SyntaxKind.EqualsExpression,
            SyntaxKind.BangEqToken => SyntaxKind.NotEqualsExpression,
            SyntaxKind.LessToken => SyntaxKind.LessThanExpression,
            SyntaxKind.LessEqToken => SyntaxKind.LessThanOrEqualsExpression,
            SyntaxKind.GreaterToken => SyntaxKind.GreaterThanExpression,
            SyntaxKind.GreaterEqToken => SyntaxKind.GreaterThanOrEqualsExpression,
            SyntaxKind.AmpersandAmpersandToken => SyntaxKind.LogicalAndExpression,
            SyntaxKind.PipePipeToken => SyntaxKind.LogicalOrExpression,
            SyntaxKind.AmpersandToken => SyntaxKind.BitwiseAndExpression,
            SyntaxKind.PipeToken => SyntaxKind.BitwiseOrExpression,
            SyntaxKind.CaretToken => SyntaxKind.ExclusiveOrExpression,
            SyntaxKind.LessLessToken => SyntaxKind.LeftShiftExpression,
            SyntaxKind.GreaterGreaterToken => SyntaxKind.RightShiftExpression,
            _ => null
        };
    }

    public static SyntaxKind? GetLiteralExpressionKind(this SyntaxKind kind)
    {
        return kind switch
        {
            SyntaxKind.NumberLiteralToken => SyntaxKind.NumberLiteralExpression,
            SyntaxKind.StringLiteralToken => SyntaxKind.StringLiteralExpression,
            SyntaxKind.CharLiteralToken => SyntaxKind.CharLiteralExpression,
            _ => null
        };
    }
}

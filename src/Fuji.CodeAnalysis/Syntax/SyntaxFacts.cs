namespace Fuji.CodeAnalysis.Syntax;

public static class SyntaxFacts
{
    public static int GetUnaryOperatorPrecedence(this SyntaxKind kind)
    {
        return kind switch
        {
            SyntaxKind.PlusToken => 11,
            SyntaxKind.MinusToken => 11,
            SyntaxKind.BangToken => 11,
            SyntaxKind.TildeToken => 11,
            _ => 0
        };
    }

    public static int GetBinaryOperatorPrecedence(this SyntaxKind kind)
    {
        return kind switch
        {
            SyntaxKind.StarToken => 10,
            SyntaxKind.SlashToken => 10,
            SyntaxKind.PlusToken => 9,
            SyntaxKind.MinusToken => 9,
            SyntaxKind.LessLessToken => 8,
            SyntaxKind.GreaterGreaterToken => 8,
            SyntaxKind.LessToken => 7,
            SyntaxKind.LessEqToken => 7,
            SyntaxKind.GreaterToken => 7,
            SyntaxKind.GreaterEqToken => 7,
            SyntaxKind.EqEqToken => 6,
            SyntaxKind.BangEqToken => 6,
            SyntaxKind.AmpersandToken => 5,
            SyntaxKind.CaretToken => 4,
            SyntaxKind.PipeToken => 3,
            SyntaxKind.AmpersandAmpersandToken => 2,
            SyntaxKind.PipePipeToken => 1,
            _ => 0
        };
    }

    public static SyntaxKind GetKeywordOrIdentifierKind(string text)
    {
        return text switch
        {
            "int" => SyntaxKind.IntKeyword,
            "int8" => SyntaxKind.Int8Keyword,
            "int16" => SyntaxKind.Int16Keyword,
            "int32" => SyntaxKind.Int32Keyword,
            "int64" => SyntaxKind.Int64Keyword,
            "uint" => SyntaxKind.UIntKeyword,
            "uint8" => SyntaxKind.UInt8Keyword,
            "uint16" => SyntaxKind.UInt16Keyword,
            "uint32" => SyntaxKind.UInt32Keyword,
            "uint64" => SyntaxKind.UInt64Keyword,
            "float" => SyntaxKind.FloatKeyword,
            "double" => SyntaxKind.DoubleKeyword,
            "bool" => SyntaxKind.BoolKeyword,
            "string" => SyntaxKind.StringKeyword,
            "char" => SyntaxKind.CharKeyword,
            "struct" => SyntaxKind.StructKeyword,
            "interface" => SyntaxKind.InterfaceKeyword,
            "func" => SyntaxKind.FuncKeyword,
            "if" => SyntaxKind.IfKeyword,
            "else" => SyntaxKind.ElseKeyword,
            "foreach" => SyntaxKind.ForeachKeyword,
            "let" => SyntaxKind.LetKeyword,
            "loop" => SyntaxKind.LoopKeyword,
            "continue" => SyntaxKind.ContinueKeyword,
            "break" => SyntaxKind.BreakKeyword,
            "return" => SyntaxKind.ReturnKeyword,
            "true" => SyntaxKind.TrueKeyword,
            "false" => SyntaxKind.FalseKeyword,
            "null" => SyntaxKind.NullKeyword,
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
            SyntaxKind.TrueKeyword => SyntaxKind.TrueLiteralExpression,
            SyntaxKind.FalseKeyword => SyntaxKind.FalseLiteralExpression,
            SyntaxKind.NullKeyword => SyntaxKind.NullLiteralExpression,
            _ => null
        };
    }

    internal static bool IsPredefinedType(SyntaxKind kind)
    {
        return kind switch
        {
            SyntaxKind.IntKeyword => true,
            SyntaxKind.Int8Keyword => true,
            SyntaxKind.Int16Keyword => true,
            SyntaxKind.Int32Keyword => true,
            SyntaxKind.Int64Keyword => true,
            SyntaxKind.UIntKeyword => true,
            SyntaxKind.UInt8Keyword => true,
            SyntaxKind.UInt16Keyword => true,
            SyntaxKind.UInt32Keyword => true,
            SyntaxKind.UInt64Keyword => true,
            SyntaxKind.FloatKeyword => true,
            SyntaxKind.DoubleKeyword => true,
            SyntaxKind.BoolKeyword => true,
            SyntaxKind.StringKeyword => true,
            SyntaxKind.CharKeyword => true,
            _ => false
        };
    }
}

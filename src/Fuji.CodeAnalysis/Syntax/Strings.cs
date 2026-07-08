namespace Fuji.CodeAnalysis.Syntax;

public static class Strings
{
    public const string Plus = "+";
    public const string Minus = "-";
    public const string Star = "*";
    public const string Slash = "/";
    public const string Eq = "=";
    public const string Bang = "!";
    public const string EqEq = "==";
    public const string BangEq = "!=";
    public const string Less = "<";
    public const string LessEq = "<=";
    public const string LessLess = "<<";
    public const string Greater = ">";
    public const string GreaterEq = ">=";
    public const string GreaterGreater = ">>";
    public const string OpenParen = "(";
    public const string CloseParen = ")";
    public const string OpenBrace = "{";
    public const string CloseBrace = "}";
    public const string OpenBracket = "[";
    public const string CloseBracket = "]";
    public const string Comma = ",";
    public const string Dot = ".";
    public const string Colon = ":";
    public const string Semicolon = ";";
    public const string Question = "?";
    public const string Ampersand = "&";
    public const string Pipe = "|";
    public const string AmpersandAmpersand = "&&";
    public const string PipePipe = "||";
    public const string Caret = "^";
    public const string Tilde = "~";

    public const string IntKeyword = "int";
    public const string FloatKeyword = "float";
    public const string BoolKeyword = "bool";
    public const string StringKeyword = "string";
    public const string CharKeyword = "char";
    public const string DoubleKeyword = "double";
    public const string Int8Keyword = "int8";
    public const string Int16Keyword = "int16";
    public const string Int32Keyword = "int32";
    public const string Int64Keyword = "int64";
    public const string UIntKeyword = "uint";
    public const string UInt8Keyword = "uint8";
    public const string UInt16Keyword = "uint16";
    public const string UInt32Keyword = "uint32";
    public const string UInt64Keyword = "uint64";
    public const string StructKeyword = "struct";
    public const string InterfaceKeyword = "interface";
    public const string FuncKeyword = "func";
    public const string LetKeyword = "let";
    public const string IfKeyword = "if";
    public const string ElseKeyword = "else";
    public const string LoopKeyword = "loop";
    public const string ForeachKeyword = "foreach";
    public const string ContinueKeyword = "continue";
    public const string BreakKeyword = "break";
    public const string ReturnKeyword = "return";
    public const string TrueKeyword = "true";
    public const string FalseKeyword = "false";
    public const string NullKeyword = "null";

    public static string GetInternedText(string text)
    {
        return text switch
        {
            "/" => Slash,
            "=" => Eq,
            "!" => Bang,
            "==" => EqEq,
            "!=" => BangEq,
            "<" => Less,
            "<=" => LessEq,
            "<<" => LessLess,
            ">" => Greater,
            ">=" => GreaterEq,
            ">>" => GreaterGreater,
            "(" => OpenParen,
            ")" => CloseParen,
            "{" => OpenBrace,
            "}" => CloseBrace,
            "[" => OpenBracket,
            "]" => CloseBracket,
            "," => Comma,
            "." => Dot,
            ":" => Colon,
            ";" => Semicolon,
            "?" => Question,
            "&" => Ampersand,
            "|" => Pipe,
            "&&" => AmpersandAmpersand,
            "||" => PipePipe,
            "^" => Caret,
            "~" => Tilde,
            "int" => IntKeyword,
            "float" => FloatKeyword,
            "bool" => BoolKeyword,
            "string" => StringKeyword,
            "char" => CharKeyword,
            "double" => DoubleKeyword,
            "int8" => Int8Keyword,
            "int16" => Int16Keyword,
            "int32" => Int32Keyword,
            "int64" => Int64Keyword,
            "uint" => UIntKeyword,
            "uint8" => UInt8Keyword,
            "uint16" => UInt16Keyword,
            "uint32" => UInt32Keyword,
            "uint64" => UInt64Keyword,
            "struct" => StructKeyword,
            "interface" => InterfaceKeyword,
            "func" => FuncKeyword,
            "let" => LetKeyword,
            "if" => IfKeyword,
            "else" => ElseKeyword,
            "loop" => LoopKeyword,
            "foreach" => ForeachKeyword,
            "continue" => ContinueKeyword,
            "break" => BreakKeyword,
            "return" => ReturnKeyword,
            "true" => TrueKeyword,
            "false" => FalseKeyword,
            "null" => NullKeyword,
            _ => text
        };
    }

}

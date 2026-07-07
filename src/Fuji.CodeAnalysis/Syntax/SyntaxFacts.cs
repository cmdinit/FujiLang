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
}

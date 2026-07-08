using Fuji.CodeAnalysis.Syntax;

namespace Fuji.CodeAnalysis.Parser;

public abstract class AbstractParser
{
    public readonly string Text;
    private readonly List<SyntaxToken> _tokens = new();
    private int _position = 0;
    private readonly int _length = 0;

    protected AbstractParser(string text)
    {
        Text = text;
        var lexer = new Lexer(text);
        while (true)
        {
            SyntaxToken token = lexer.Lex();
            _tokens.Add(token);
            if (token.Kind == SyntaxKind.EofToken)
            {
                break;
            }
        }
        _length = _tokens.Count;
    }

    protected SyntaxToken Peek(int offset)
    {
        int index = _position + offset;
        if (index >= _length)
            return _tokens[_length - 1];

        return _tokens[index];
    }

    protected SyntaxToken Current => Peek(0);
    protected SyntaxToken LookAhead => Peek(1);

    protected SyntaxToken EatToken()
    {
        var token = Current;
        _position++;
        return token;
    }

    protected void Advance()
    {
        _position++;
    }

    protected SyntaxToken MatchToken(SyntaxKind kind)
    {
        if (Current.Kind == kind)
        {
            return EatToken();
        }

        return new SyntaxToken(kind, "", Current.Start, 0);
    }

    protected bool IsAtEnd => _position >= _length - 1;
}

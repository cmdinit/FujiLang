using Fuji.CodeAnalysis.Syntax;
using Fuji.CodeAnalysis.Text;

namespace Fuji.CodeAnalysis.Parser;

public class Lexer
{
    private readonly SourceText _st;

    public Lexer(string text)
    {
        _st = new SourceText(text);
    }

    public SyntaxToken Lex()
    {
        LexTrivia(out var leadingTriviaLength);
        LexToken(out var kind, out var text, out var start);

        return new SyntaxToken(kind, text, start, leadingTriviaLength);
    }

    private void LexToken(out SyntaxKind kind, out string text, out int start)
    {
        kind = SyntaxKind.BadToken;
        text = string.Empty;
        start = _st.Position;

        if (_st.IsAtEnd)
        {
            kind = SyntaxKind.EofToken;
            text = '\0'.ToString();
            return;
        }

        if (char.IsLetter(_st.Current) || _st.Current == '_')
        {
            while (char.IsLetterOrDigit(_st.Current) || _st.Current == '_')
            {
                _st.Eat();
            }

            text = _st.GetText();
            kind = SyntaxFacts.GetKeywordOrIdentifierKind(text);
            return;
        }

        if (char.IsDigit(_st.Current))
        {
            while (char.IsDigit(_st.Current))
            {
                _st.Eat();
            }
            if (_st.Current == '.')
            {
                _st.Eat();
                while (char.IsDigit(_st.Current))
                {
                    _st.Eat();
                }
            }

            text = _st.GetText();
            kind = SyntaxKind.NumberLiteralToken;
            return;
        }

        switch (_st.Current)
        {
            case '+':
                kind = SyntaxKind.PlusToken;
                text = Strings.Plus;
                _st.Eat();
                break;
            case '-':
                kind = SyntaxKind.MinusToken;
                text = Strings.Minus;
                _st.Eat();
                break;
            case '*':
                kind = SyntaxKind.StarToken;
                text = Strings.Star;
                _st.Eat();
                break;
            case '/':
                kind = SyntaxKind.SlashToken;
                text = Strings.Slash;
                _st.Eat();
                break;
            case '(':
                kind = SyntaxKind.OpenParenToken;
                text = Strings.OpenParen;
                _st.Eat();
                break;
            case ')':
                kind = SyntaxKind.CloseParenToken;
                text = Strings.CloseParen;
                _st.Eat();
                break;
            case '{':
                kind = SyntaxKind.OpenBraceToken;
                text = Strings.OpenBrace;
                _st.Eat();
                break;
            case '}':
                kind = SyntaxKind.CloseBraceToken;
                text = Strings.CloseBrace;
                _st.Eat();
                break;
            case '[':
                kind = SyntaxKind.OpenBracketToken;
                text = Strings.OpenBracket;
                _st.Eat();
                break;
            case ']':
                kind = SyntaxKind.CloseBracketToken;
                text = Strings.CloseBracket;
                _st.Eat();
                break;
            case ',':
                kind = SyntaxKind.CommaToken;
                text = Strings.Comma;
                _st.Eat();
                break;
            case '.':
                kind = SyntaxKind.DotToken;
                text = Strings.Dot;
                _st.Eat();
                break;
            case ':':
                kind = SyntaxKind.ColonToken;
                text = Strings.Colon;
                _st.Eat();
                break;
            case '>':
                if (_st.LookAhead == '=')
                {
                    kind = SyntaxKind.GreaterEqToken;
                    text = Strings.GreaterEq;
                    _st.Eat(2);
                }
                else if (_st.LookAhead == '>')
                {
                    kind = SyntaxKind.GreaterGreaterToken;
                    text = Strings.GreaterGreater;
                    _st.Eat(2);
                }
                else
                {
                    kind = SyntaxKind.GreaterToken;
                    text = Strings.Greater;
                    _st.Eat();
                }
                break;
            case '<':
                if (_st.LookAhead == '=')
                {
                    kind = SyntaxKind.LessEqToken;
                    text = Strings.LessEq;
                    _st.Eat(2);
                }
                else if (_st.LookAhead == '<')
                {
                    kind = SyntaxKind.LessLessToken;
                    text = Strings.LessLess;
                    _st.Eat(2);
                }
                else
                {
                    kind = SyntaxKind.LessToken;
                    text = Strings.Less;
                    _st.Eat();
                }
                break;
            case '=':
                if (_st.LookAhead == '=')
                {
                    kind = SyntaxKind.EqEqToken;
                    text = Strings.EqEq;
                    _st.Eat(2);
                }
                else
                {
                    kind = SyntaxKind.EqToken;
                    text = Strings.Eq;
                    _st.Eat();
                }
                break;
            case '!':
                if (_st.LookAhead == '=')
                {
                    kind = SyntaxKind.BangEqToken;
                    text = Strings.BangEq;
                    _st.Eat(2);
                }
                else
                {
                    kind = SyntaxKind.BangToken;
                    text = Strings.Bang;
                    _st.Eat();
                }
                break;
            case '&':
                if (_st.LookAhead == '&')
                {
                    kind = SyntaxKind.AmpersandAmpersandToken;
                    text = Strings.AmpersandAmpersand;
                    _st.Eat(2);
                }
                else
                {
                    kind = SyntaxKind.AmpersandToken;
                    text = Strings.Ampersand;
                    _st.Eat();
                }
                break;
            case '|':
                if (_st.LookAhead == '|')
                {
                    kind = SyntaxKind.PipePipeToken;
                    text = Strings.PipePipe;
                    _st.Eat(2);
                }
                else
                {
                    kind = SyntaxKind.PipeToken;
                    text = Strings.Pipe;
                    _st.Eat();
                }
                break;
            case '^':
                kind = SyntaxKind.CaretToken;
                text = Strings.Caret;
                _st.Eat();
                break;
            case '~':
                kind = SyntaxKind.TildeToken;
                text = Strings.Tilde;
                _st.Eat();
                break;
            case '?':
                kind = SyntaxKind.QuestionToken;
                text = Strings.Question;
                _st.Eat();
                break;
            case ';':
                kind = SyntaxKind.SemicolonToken;
                text = Strings.Semicolon;
                _st.Eat();
                break;
            default:
                kind = SyntaxKind.BadToken;
                _st.Eat();
                text = _st.GetText();
                break;
        }
    }

    private void LexTrivia(out int triviaLength)
    {
        triviaLength = 0;

        while (true)
        {
            if (_st.Current is '\n' or '\r' or ' ' or '\t' or ' ')
            {
                triviaLength++;
                _st.Eat();
            }
            else if (_st.Current == '/' && _st.LookAhead == '/')
            {
                triviaLength += 2;
                _st.Eat(2);

                while (_st.Current != '\n' && _st.Current != '\r' && !_st.IsAtEnd)
                {
                    triviaLength++;
                    _st.Eat();
                }
            }
            else if (_st.Current == '/' && _st.LookAhead == '*')
            {
                triviaLength += 2;
                _st.Eat(2);

                while (!(_st.Current == '*' && _st.LookAhead == '/') && !_st.IsAtEnd)
                {
                    triviaLength++;
                    _st.Eat();
                }

                if (!_st.IsAtEnd)
                {
                    triviaLength += 2;
                    _st.Eat(2);
                }
            }
            else
            {
                break;
            }
        }
    }
}

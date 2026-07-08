using Fuji.CodeAnalysis.Syntax;

namespace Fuji.CodeAnalysis.Parser;

public class LanguageParser : AbstractParser
{
    public LanguageParser(string text) : base(text)
    {
    }

    public CompilationUnitSyntax ParseCompilationUnit()
    {
        var members = ParseRootDeclarations();
        var eofToken = MatchToken(SyntaxKind.EofToken);

        return CompilationUnitSyntax.Create(members, eofToken);
    }

    public RootDeclarationListSyntax ParseRootDeclarations()
    {
        var builder = RootDeclarationListSyntax.GetBuilder();

        while (true)
        {
            if (Current.Kind == SyntaxKind.EofToken)
            {
                break;
            }

            var member = ParseRootDeclaration();
            if (member == null)
            {
                break;
            }

            builder.Add(member);
        }

        return builder.Build();
    }

    public RootDeclarationSyntax? ParseRootDeclaration()
    {
        if (Current.Kind == SyntaxKind.FuncKeyword)
        {
            return ParseFunctionDeclaration();
        }
        else if (Current.Kind == SyntaxKind.StructKeyword)
        {
            return ParseStructDeclaration();
        }

        return null;
    }

    private StructDeclarationSyntax ParseStructDeclaration()
    {
        var structKeyword = MatchToken(SyntaxKind.StructKeyword);
        var identifier = MatchToken(SyntaxKind.IdentifierToken);
        var openBraceToken = MatchToken(SyntaxKind.OpenBraceToken);
        var members = ParseMemberDeclarations();
        var closeBraceToken = MatchToken(SyntaxKind.CloseBraceToken);

        return StructDeclarationSyntax.Create(structKeyword, identifier, openBraceToken, members, closeBraceToken);
    }

    private FunctionDeclarationSyntax ParseFunctionDeclaration()
    {
        var funcKeyword = MatchToken(SyntaxKind.FuncKeyword);
        var identifier = MatchToken(SyntaxKind.IdentifierToken);
        var openParenToken = MatchToken(SyntaxKind.OpenParenToken);
        var parameters = ParseParameterList();
        var closeParenToken = MatchToken(SyntaxKind.CloseParenToken);
        var body = ParseBlock();

        return FunctionDeclarationSyntax.Create(funcKeyword, identifier, openParenToken, parameters, closeParenToken, body);
    }

    private ParameterListSyntax ParseParameterList()
    {
        var builder = ParameterListSyntax.GetBuilder();

        while (true)
        {
            if (Current.Kind == SyntaxKind.CloseParenToken || Current.Kind == SyntaxKind.EofToken)
            {
                break;
            }

            var parameter = ParseParameter();
            if (parameter == null)
            {
                break;
            }

            builder.AddNode(parameter);

            if (Current.Kind == SyntaxKind.CommaToken)
            {
                var commaToken = EatToken();
                builder.AddSeparator(commaToken);
            }
        }

        return builder.Build();
    }

    private ParameterSyntax ParseParameter()
    {
        var identifier = MatchToken(SyntaxKind.IdentifierToken);
        var colonToken = MatchToken(SyntaxKind.ColonToken);
        var type = ParseType();

        return ParameterSyntax.Create(identifier, colonToken, type);
    }

    private ArgumentListSyntax ParseArgumentList()
    {
        var builder = ArgumentListSyntax.GetBuilder();

        while (true)
        {
            if (Current.Kind == SyntaxKind.CloseParenToken || Current.Kind == SyntaxKind.EofToken)
            {
                break;
            }

            var argument = ParseArgument();
            if (argument == null)
            {
                break;
            }

            builder.AddNode(argument);

            if (Current.Kind == SyntaxKind.CommaToken)
            {
                var commaToken = EatToken();
                builder.AddSeparator(commaToken);
            }
        }

        return builder.Build();
    }

    private ArgumentSyntax ParseArgument()
    {
        var identifier = MatchToken(SyntaxKind.IdentifierToken);
        var colonToken = MatchToken(SyntaxKind.ColonToken);
        var expression = ParseExpression();
        return ArgumentSyntax.Create(identifier, colonToken, expression);
    }

    private ExpressionSyntax ParseExpression()
    {
        return ParseBinaryExpression();
    }

    private ExpressionSyntax ParseBinaryExpression(int parentPrecedence = 0)
    {
        var left = ParseUnaryOrPostfixExpression();

        while (true)
        {
            var precedence = Current.Kind.GetBinaryOperatorPrecedence();
            if (precedence == 0 || precedence <= parentPrecedence)
            {
                break;
            }

            var operatorToken = EatToken();
            var right = ParseBinaryExpression(precedence);
            left = BinaryExpressionSyntax.Create(left, operatorToken, right);
        }

        return left;
    }

    private ExpressionSyntax ParseUnaryOrPostfixExpression()
    {
        var unaryPrecedence = Current.Kind.GetUnaryOperatorPrecedence();
        if (unaryPrecedence != 0)
        {
            var operatorToken = EatToken();
            var operand = ParseUnaryOrPostfixExpression();
            return PrefixUnaryExpressionSyntax.Create(operatorToken, operand);
        }

        var term = ParseTermExpression();
        return ParsePostfixExpression(term);
    }

    private ExpressionSyntax ParsePostfixExpression(ExpressionSyntax expression)
    {
        while (true)
        {
            if (Current.Kind == SyntaxKind.DotToken)
            {
                var dotToken = EatToken();
                var identifierToken = MatchToken(SyntaxKind.IdentifierToken);
                expression = new MemberAccessExpressionSyntax(expression, dotToken, identifierToken);
                continue;
            }

            if (Current.Kind == SyntaxKind.OpenParenToken)
            {
                var openParenToken = EatToken();
                var argumentList = ParseArgumentList();
                var closeParenToken = MatchToken(SyntaxKind.CloseParenToken);
                expression = new InvocationExpressionSyntax(expression, openParenToken, argumentList, closeParenToken);
                continue;
            }

            break;
        }

        return expression;
    }

    private ExpressionSyntax ParseTermExpression()
    {
        if (Current.Kind == SyntaxKind.OpenParenToken)
        {
            var openParenToken = EatToken();
            var expression = ParseExpression();
            var closeParenToken = MatchToken(SyntaxKind.CloseParenToken);
            return new ParenthesisExpressionSyntax(openParenToken, expression, closeParenToken);
        }

        if (Current.Kind.GetLiteralExpressionKind() != null)
        {
            var literalToken = EatToken();
            return LiteralExpressionSyntax.Create(literalToken);
        }

        if (Current.Kind == SyntaxKind.IdentifierToken)
        {
            var identifierToken = EatToken();
            return IdentifierNameSyntax.Create(identifierToken);
        }

        var missingIdentifierToken = MatchToken(SyntaxKind.IdentifierToken);
        return IdentifierNameSyntax.Create(missingIdentifierToken);
    }

    private BlockSyntax ParseBlock()
    {
        var openBraceToken = MatchToken(SyntaxKind.OpenBraceToken);
        var statements = ParseStatements();
        var closeBraceToken = MatchToken(SyntaxKind.CloseBraceToken);

        return BlockSyntax.Create(openBraceToken, statements, closeBraceToken);
    }

    private StatementListSyntax ParseStatements()
    {
        var builder = StatementListSyntax.GetBuilder();

        while (true)
        {
            if (Current.Kind == SyntaxKind.CloseBraceToken || Current.Kind == SyntaxKind.EofToken)
            {
                break;
            }

            var statement = ParseStatement();
            if (statement == null)
            {
                break;
            }

            builder.Add(statement);
        }

        return builder.Build();
    }

    private StatementSyntax? ParseStatement()
    {
        return Current.Kind switch
        {
            SyntaxKind.OpenBraceToken => ParseBlock(),
            SyntaxKind.IfKeyword => ParseIfStatement(),
            SyntaxKind.LoopKeyword => ParseLoopStatement(),
            SyntaxKind.ForeachKeyword => ParseForeachStatement(),
            SyntaxKind.ReturnKeyword => ParseReturnStatement(),
            SyntaxKind.BreakKeyword => ParseBreakStatement(),
            SyntaxKind.ContinueKeyword => ParseContinueStatement(),
            SyntaxKind.SemicolonToken => ParseEmptyStatement(),
            _ => ParseExpressionStatement()
        };
    }

    private StatementSyntax ParseIfStatement()
    {
        var ifKeyword = MatchToken(SyntaxKind.IfKeyword);
        var openParenToken = MatchToken(SyntaxKind.OpenParenToken);
        var condition = ParseExpression();
        var closeParenToken = MatchToken(SyntaxKind.CloseParenToken);
        var body = ParseStatement() ?? ParseEmptyStatement();

        ElseClauseSyntax? elseClause = null;
        if (Current.Kind == SyntaxKind.ElseKeyword)
        {
            var elseKeyword = EatToken();
            var elseBody = ParseStatement() ?? ParseEmptyStatement();
            elseClause = ElseClauseSyntax.Create(elseKeyword, elseBody);
        }

        return IfStatementSyntax.Create(ifKeyword, openParenToken, condition, closeParenToken, body, elseClause);
    }

    private StatementSyntax ParseLoopStatement()
    {
        var loopKeyword = MatchToken(SyntaxKind.LoopKeyword);
        var body = ParseStatement() ?? ParseEmptyStatement();
        return LoopStatementSyntax.Create(loopKeyword, body);
    }

    private StatementSyntax ParseForeachStatement()
    {
        var foreachKeyword = MatchToken(SyntaxKind.ForeachKeyword);
        var openParenToken = MatchToken(SyntaxKind.OpenParenToken);
        var identifierToken = MatchToken(SyntaxKind.IdentifierToken);
        var colonToken = MatchToken(SyntaxKind.ColonToken);
        var expression = ParseExpression();
        var closeParenToken = MatchToken(SyntaxKind.CloseParenToken);
        var body = ParseStatement() ?? ParseEmptyStatement();

        return ForeachStatementSyntax.Create(
            foreachKeyword,
            openParenToken,
            identifierToken,
            colonToken,
            expression,
            closeParenToken,
            body);
    }

    private StatementSyntax ParseReturnStatement()
    {
        var returnKeyword = MatchToken(SyntaxKind.ReturnKeyword);

        ExpressionSyntax? expression = null;
        if (Current.Kind != SyntaxKind.SemicolonToken)
        {
            expression = ParseExpression();
        }

        var semicolonToken = MatchToken(SyntaxKind.SemicolonToken);
        return ReturnStatementSyntax.Create(returnKeyword, expression, semicolonToken);
    }

    private StatementSyntax ParseBreakStatement()
    {
        var breakKeyword = MatchToken(SyntaxKind.BreakKeyword);
        var semicolonToken = MatchToken(SyntaxKind.SemicolonToken);
        return BreakStatementSyntax.Create(breakKeyword, semicolonToken);
    }

    private StatementSyntax ParseContinueStatement()
    {
        var continueKeyword = MatchToken(SyntaxKind.ContinueKeyword);
        var semicolonToken = MatchToken(SyntaxKind.SemicolonToken);
        return ContinueStatementSyntax.Create(continueKeyword, semicolonToken);
    }

    private StatementSyntax ParseEmptyStatement()
    {
        var semicolonToken = MatchToken(SyntaxKind.SemicolonToken);
        return EmptyStatementSyntax.Create(semicolonToken);
    }

    private StatementSyntax ParseExpressionStatement()
    {
        var expression = ParseExpression();
        var semicolonToken = MatchToken(SyntaxKind.SemicolonToken);
        return ExpressionStatementSyntax.Create(expression, semicolonToken);
    }

    private MemberDeclarationListSyntax ParseMemberDeclarations()
    {
        var builder = MemberDeclarationListSyntax.GetBuilder();

        while (true)
        {
            if (Current.Kind == SyntaxKind.CloseBraceToken || Current.Kind == SyntaxKind.EofToken)
            {
                break;
            }

            var member = ParseMemberDeclaration();
            if (member == null)
            {
                break;
            }

            // Add member to the list
            builder.Add(member);
        }

        return builder.Build();
    }

    private MemberDeclarationSyntax? ParseMemberDeclaration()
    {
        while (true)
        {
            if (Current.Kind == SyntaxKind.EofToken || Current.Kind == SyntaxKind.CloseBraceToken)
            {
                return null;
            }

            if (Current.Kind == SyntaxKind.IdentifierToken)
            {
                return ParseFieldDeclaration();
            }

            // If we reach here, it means we encountered an unexpected token.
            // We can choose to either skip it or throw an error. For now, let's skip it.
            return null;
        }
    }

    private FieldDeclarationSyntax ParseFieldDeclaration()
    {
        var identifier = MatchToken(SyntaxKind.IdentifierToken);
        var colonToken = MatchToken(SyntaxKind.ColonToken);
        var type = ParseType();
        var semicolonToken = MatchToken(SyntaxKind.SemicolonToken);

        return FieldDeclarationSyntax.Create(identifier, colonToken, type, semicolonToken);
    }

    private TypeSyntax ParseType()
    {
        if (SyntaxFacts.IsPredefinedType(Current.Kind))
        {
            var typeToken = EatToken();
            return PredefinedTypeSyntax.Create(typeToken);
        }

        var identifierToken = MatchToken(SyntaxKind.IdentifierToken);
        return IdentifierNameSyntax.Create(identifierToken);
    }
}
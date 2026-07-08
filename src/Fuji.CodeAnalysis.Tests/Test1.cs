namespace Fuji.CodeAnalysis.Tests;

using System.Reflection;
using Fuji.CodeAnalysis.Parser;
using Fuji.CodeAnalysis.Semantics;
using Fuji.CodeAnalysis.Symbols;
using Fuji.CodeAnalysis.Syntax;

[TestClass]
public sealed class Test1
{
    [TestMethod]
    public void ParseCompilationUnit_ParsesStructBlockWithFields()
    {
        var source = """
            struct Point {
                x: Number;
                y: Number;
            }

            func hello()
            {
            }
            """;

        var parser = new LanguageParser(source);

        var unit = parser.ParseCompilationUnit();

        Assert.AreEqual(SyntaxKind.CompilationUnit, unit.Kind);
        Assert.AreEqual(2, unit.Declarations.SlotCount);
        Assert.AreEqual(SyntaxKind.EofToken, unit.EofToken.Kind);

        var structDeclaration = unit.Declarations.GetSlot(0) as StructDeclarationSyntax;
        Assert.IsNotNull(structDeclaration);
        Assert.AreEqual("Point", structDeclaration.Identifier.Text);
        Assert.AreEqual(2, structDeclaration.Declarations.SlotCount);

        var firstField = structDeclaration.Declarations.GetSlot(0) as FieldDeclarationSyntax;
        var secondField = structDeclaration.Declarations.GetSlot(1) as FieldDeclarationSyntax;

        Assert.IsNotNull(firstField);
        Assert.IsNotNull(secondField);
        Assert.AreEqual("x", firstField.Identifier.Text);
        Assert.AreEqual("y", secondField.Identifier.Text);
        Assert.AreEqual(SyntaxKind.SemicolonToken, firstField.SemicolonToken.Kind);
        Assert.AreEqual(SyntaxKind.SemicolonToken, secondField.SemicolonToken.Kind);

        var functionDeclaration = unit.Declarations.GetSlot(1) as FunctionDeclarationSyntax;
        Assert.IsNotNull(functionDeclaration);
        Assert.AreEqual("hello", functionDeclaration.Identifier.Text);
        Assert.AreEqual(SyntaxKind.OpenParenToken, functionDeclaration.OpenParenToken.Kind);
        Assert.AreEqual(SyntaxKind.CloseParenToken, functionDeclaration.CloseParenToken.Kind);
        Assert.AreEqual(SyntaxKind.Block, functionDeclaration.Body.Kind);
        Assert.AreEqual(0, functionDeclaration.Body.Statements.SlotCount);
    }

    [TestMethod]
    public void ParseExpression_MultiplicationBindsTighterThanAddition()
    {
        var expression = ParseExpression("a + b * c");

        var add = AssertIsBinary(expression, SyntaxKind.AddExpression);
        AssertIsIdentifier(add.Left, "a");

        var multiply = AssertIsBinary(add.Right, SyntaxKind.MultiplyExpression);
        AssertIsIdentifier(multiply.Left, "b");
        AssertIsIdentifier(multiply.Right, "c");
    }

    [TestMethod]
    public void ParseExpression_ParenthesizedExpressionOverridesBinaryPrecedence()
    {
        var expression = ParseExpression("(a + b) * c");

        var multiply = AssertIsBinary(expression, SyntaxKind.MultiplyExpression);
        var parenthesized = Assert.IsInstanceOfType<ParenthesisExpressionSyntax>(multiply.Left);
        var add = AssertIsBinary(parenthesized.Expression, SyntaxKind.AddExpression);

        AssertIsIdentifier(add.Left, "a");
        AssertIsIdentifier(add.Right, "b");
        AssertIsIdentifier(multiply.Right, "c");
    }

    [TestMethod]
    public void ParseExpression_UnaryBindsTighterThanMultiplication()
    {
        var expression = ParseExpression("-a * b");

        var multiply = AssertIsBinary(expression, SyntaxKind.MultiplyExpression);
        var unary = Assert.IsInstanceOfType<PrefixUnaryExpressionSyntax>(multiply.Left);
        Assert.AreEqual(SyntaxKind.UnaryMinusExpression, unary.Kind);
        AssertIsIdentifier(unary.Operand, "a");
        AssertIsIdentifier(multiply.Right, "b");
    }

    [TestMethod]
    public void ParseExpression_PostfixBindsTighterThanBinary()
    {
        var expression = ParseExpression("a.b + c");

        var add = AssertIsBinary(expression, SyntaxKind.AddExpression);
        var memberAccess = Assert.IsInstanceOfType<MemberAccessExpressionSyntax>(add.Left);
        AssertIsIdentifier(memberAccess.Expression, "a");
        Assert.AreEqual("b", memberAccess.IdentifierToken.Text);
        AssertIsIdentifier(add.Right, "c");
    }

    [TestMethod]
    public void ParseExpression_PostfixChainBindsBeforeBinary()
    {
        var expression = ParseExpression("a.b(x: c).d + e");

        var add = AssertIsBinary(expression, SyntaxKind.AddExpression);
        var finalMemberAccess = Assert.IsInstanceOfType<MemberAccessExpressionSyntax>(add.Left);
        Assert.AreEqual("d", finalMemberAccess.IdentifierToken.Text);

        var invocation = Assert.IsInstanceOfType<InvocationExpressionSyntax>(finalMemberAccess.Expression);
        var targetMemberAccess = Assert.IsInstanceOfType<MemberAccessExpressionSyntax>(invocation.Expression);
        AssertIsIdentifier(targetMemberAccess.Expression, "a");
        Assert.AreEqual("b", targetMemberAccess.IdentifierToken.Text);
        Assert.AreEqual(1, invocation.Arguments.SlotCount);

        var argument = Assert.IsInstanceOfType<ArgumentSyntax>(invocation.Arguments.GetSlot(0));
        Assert.AreEqual("x", argument.Identifier.Text);
        AssertIsIdentifier(argument.Expression, "c");
        AssertIsIdentifier(add.Right, "e");
    }

    [TestMethod]
    public void ParseExpression_AndBindsTighterThanOr()
    {
        var expression = ParseExpression("a || b && c");

        var logicalOr = AssertIsBinary(expression, SyntaxKind.LogicalOrExpression);
        AssertIsIdentifier(logicalOr.Left, "a");

        var logicalAnd = AssertIsBinary(logicalOr.Right, SyntaxKind.LogicalAndExpression);
        AssertIsIdentifier(logicalAnd.Left, "b");
        AssertIsIdentifier(logicalAnd.Right, "c");
    }

    [TestMethod]
    public void ParseFunctionBody_ParsesAllRequestedStatementKinds()
    {
        var source = """
            func main() {
                ;
                if (cond) { ; } else continue;
                loop break;
                foreach (item: items) continue;
                return value;
                break;
                continue;
            }
            """;

        var parser = new LanguageParser(source);
        var unit = parser.ParseCompilationUnit();

        var function = Assert.IsInstanceOfType<FunctionDeclarationSyntax>(unit.Declarations.GetSlot(0));
        var statements = function.Body.Statements;

        Assert.AreEqual(7, statements.SlotCount);

        Assert.IsInstanceOfType<EmptyStatementSyntax>(statements.GetSlot(0));

        var ifStatement = Assert.IsInstanceOfType<IfStatementSyntax>(statements.GetSlot(1));
        Assert.IsInstanceOfType<IdentifierNameSyntax>(ifStatement.Condition);
        Assert.IsInstanceOfType<BlockSyntax>(ifStatement.Body);
        Assert.IsNotNull(ifStatement.ElseClause);
        Assert.IsInstanceOfType<ContinueStatementSyntax>(ifStatement.ElseClause.Statement);

        var loopStatement = Assert.IsInstanceOfType<LoopStatementSyntax>(statements.GetSlot(2));
        Assert.IsInstanceOfType<BreakStatementSyntax>(loopStatement.Body);

        var foreachStatement = Assert.IsInstanceOfType<ForeachStatementSyntax>(statements.GetSlot(3));
        Assert.AreEqual("item", foreachStatement.IdentifierToken.Text);
        Assert.IsInstanceOfType<IdentifierNameSyntax>(foreachStatement.Expression);
        Assert.IsInstanceOfType<ContinueStatementSyntax>(foreachStatement.Body);

        var returnStatement = Assert.IsInstanceOfType<ReturnStatementSyntax>(statements.GetSlot(4));
        Assert.IsNotNull(returnStatement.Expression);

        Assert.IsInstanceOfType<BreakStatementSyntax>(statements.GetSlot(5));
        Assert.IsInstanceOfType<ContinueStatementSyntax>(statements.GetSlot(6));
    }

    [TestMethod]
    public void ParseForeachStatement_ParsesHeaderAndBody()
    {
        var source = """
            func main() {
                foreach (entry: source)
                {
                    ;
                }
            }
            """;

        var parser = new LanguageParser(source);
        var unit = parser.ParseCompilationUnit();
        var function = Assert.IsInstanceOfType<FunctionDeclarationSyntax>(unit.Declarations.GetSlot(0));
        var foreachStatement = Assert.IsInstanceOfType<ForeachStatementSyntax>(function.Body.Statements.GetSlot(0));

        Assert.AreEqual("entry", foreachStatement.IdentifierToken.Text);
        Assert.IsInstanceOfType<IdentifierNameSyntax>(foreachStatement.Expression);
        Assert.IsInstanceOfType<BlockSyntax>(foreachStatement.Body);
    }

    [TestMethod]
    public void Compilation_BindsSourceSymbolsForDeclarations()
    {
        var source = """
            struct Point {
                x: int;
                y: Unknown;
            }

            func move(dx: int, point: Point) {
            }
            """;

        var syntaxTree = new ParsedSyntaxTree(new Fuji.CodeAnalysis.Text.SourceText(source));
        var compilation = Compilation.Create([syntaxTree]);
        var model = compilation.GetSemanticModel(syntaxTree);

        Assert.AreEqual(1, model.NamedTypes.Count);
        Assert.AreEqual(1, model.Methods.Count);

        var structDecl = Assert.IsInstanceOfType<StructDeclarationSyntax>(syntaxTree.Root.Declarations.GetSlot(0));
        var typeSymbol = Assert.IsInstanceOfType<INamedTypeSymbol>(model.GetDeclaredSymbol(structDecl));
        Assert.AreEqual("Point", typeSymbol.Name);
        Assert.AreEqual(2, typeSymbol.Fields.Count);

        var firstField = typeSymbol.Fields[0];
        var secondField = typeSymbol.Fields[1];
        Assert.AreEqual("x", firstField.Name);
        Assert.AreEqual("int", firstField.Type.Name);
        Assert.AreEqual(TypeKind.Builtin, firstField.Type.TypeKind);

        Assert.AreEqual("y", secondField.Name);
        Assert.AreEqual(TypeKind.Error, secondField.Type.TypeKind);
        Assert.IsInstanceOfType<IErrorTypeSymbol>(secondField.Type);

        var functionDecl = Assert.IsInstanceOfType<FunctionDeclarationSyntax>(syntaxTree.Root.Declarations.GetSlot(1));
        var methodSymbol = Assert.IsInstanceOfType<IMethodSymbol>(model.GetDeclaredSymbol(functionDecl));
        Assert.AreEqual("move", methodSymbol.Name);
        Assert.AreEqual(2, methodSymbol.Parameters.Count);
        Assert.AreEqual("int", methodSymbol.Parameters[0].Type.Name);
        Assert.AreEqual("Point", methodSymbol.Parameters[1].Type.Name);

        var parameterDecl = Assert.IsInstanceOfType<ParameterSyntax>(functionDecl.Parameters.GetSlot(0));
        var parameterSymbol = Assert.IsInstanceOfType<IParameterSymbol>(model.GetDeclaredSymbol(parameterDecl));
        Assert.AreEqual(0, parameterSymbol.Ordinal);
        Assert.AreEqual("dx", parameterSymbol.Name);
    }

    [TestMethod]
    public void SemanticModel_ResolvesExpressionSymbolAndTypeInfo()
    {
        var source = """
            struct Point {
                x: int;
            }

            func move(p: Point) {
                p;
                p.x;
                move(p: p);
                missing;
                "hello";
                1;
                true;
            }
            """;

        var syntaxTree = new ParsedSyntaxTree(new Fuji.CodeAnalysis.Text.SourceText(source));
        var compilation = Compilation.Create([syntaxTree]);
        var model = compilation.GetSemanticModel(syntaxTree);

        var functionDecl = Assert.IsInstanceOfType<FunctionDeclarationSyntax>(syntaxTree.Root.Declarations.GetSlot(1));

        var statement0 = Assert.IsInstanceOfType<ExpressionStatementSyntax>(functionDecl.Body.Statements.GetSlot(0));
        var pIdentifier = Assert.IsInstanceOfType<IdentifierNameSyntax>(statement0.Expression);
        var pSymbol = Assert.IsInstanceOfType<IParameterSymbol>(model.GetSymbolInfo(pIdentifier));
        Assert.AreEqual("p", pSymbol.Name);
        Assert.AreEqual("Point", model.GetTypeInfo(pIdentifier).Name);

        var statement1 = Assert.IsInstanceOfType<ExpressionStatementSyntax>(functionDecl.Body.Statements.GetSlot(1));
        var memberAccess = Assert.IsInstanceOfType<MemberAccessExpressionSyntax>(statement1.Expression);
        var fieldSymbol = Assert.IsInstanceOfType<IFieldSymbol>(model.GetSymbolInfo(memberAccess));
        Assert.AreEqual("x", fieldSymbol.Name);
        Assert.AreEqual("int", model.GetTypeInfo(memberAccess).Name);

        var statement2 = Assert.IsInstanceOfType<ExpressionStatementSyntax>(functionDecl.Body.Statements.GetSlot(2));
        var invocation = Assert.IsInstanceOfType<InvocationExpressionSyntax>(statement2.Expression);
        var methodSymbol = Assert.IsInstanceOfType<IMethodSymbol>(model.GetSymbolInfo(invocation));
        Assert.AreEqual("move", methodSymbol.Name);

        var statement3 = Assert.IsInstanceOfType<ExpressionStatementSyntax>(functionDecl.Body.Statements.GetSlot(3));
        var missingIdentifier = Assert.IsInstanceOfType<IdentifierNameSyntax>(statement3.Expression);
        Assert.IsNull(model.GetSymbolInfo(missingIdentifier));
        Assert.AreEqual(TypeKind.Error, model.GetTypeInfo(missingIdentifier).TypeKind);

        var statement4 = Assert.IsInstanceOfType<ExpressionStatementSyntax>(functionDecl.Body.Statements.GetSlot(4));
        var stringLiteral = Assert.IsInstanceOfType<LiteralExpressionSyntax>(statement4.Expression);
        Assert.AreEqual("string", model.GetTypeInfo(stringLiteral).Name);

        var statement5 = Assert.IsInstanceOfType<ExpressionStatementSyntax>(functionDecl.Body.Statements.GetSlot(5));
        var numberLiteral = Assert.IsInstanceOfType<LiteralExpressionSyntax>(statement5.Expression);
        Assert.AreEqual("int", model.GetTypeInfo(numberLiteral).Name);

        var statement6 = Assert.IsInstanceOfType<ExpressionStatementSyntax>(functionDecl.Body.Statements.GetSlot(6));
        var boolLiteral = Assert.IsInstanceOfType<LiteralExpressionSyntax>(statement6.Expression);
        Assert.AreEqual("bool", model.GetTypeInfo(boolLiteral).Name);
    }

    [TestMethod]
    public void SemanticModel_ResolvesTypeSyntaxInfo()
    {
        var source = """
            struct Point {
                x: int;
            }

            func move(p: Point) {
            }
            """;

        var syntaxTree = new ParsedSyntaxTree(new Fuji.CodeAnalysis.Text.SourceText(source));
        var compilation = Compilation.Create([syntaxTree]);
        var model = compilation.GetSemanticModel(syntaxTree);

        var structDecl = Assert.IsInstanceOfType<StructDeclarationSyntax>(syntaxTree.Root.Declarations.GetSlot(0));
        var fieldDecl = Assert.IsInstanceOfType<FieldDeclarationSyntax>(structDecl.Declarations.GetSlot(0));
        var builtinType = model.GetTypeInfo(fieldDecl.Type);
        Assert.AreEqual("int", builtinType.Name);

        var functionDecl = Assert.IsInstanceOfType<FunctionDeclarationSyntax>(syntaxTree.Root.Declarations.GetSlot(1));
        var parameterDecl = Assert.IsInstanceOfType<ParameterSyntax>(functionDecl.Parameters.GetSlot(0));
        var namedType = Assert.IsInstanceOfType<INamedTypeSymbol>(model.GetSymbolInfo(parameterDecl.Type));
        Assert.AreEqual("Point", namedType.Name);
    }

    private static ExpressionSyntax ParseExpression(string source)
    {
        var parser = new LanguageParser(source);
        var parseExpressionMethod = typeof(LanguageParser).GetMethod("ParseExpression", BindingFlags.Instance | BindingFlags.NonPublic);

        Assert.IsNotNull(parseExpressionMethod, "Expected private ParseExpression method to exist.");

        var expression = parseExpressionMethod.Invoke(parser, null) as ExpressionSyntax;
        Assert.IsNotNull(expression);

        return expression;
    }

    private static BinaryExpressionSyntax AssertIsBinary(ExpressionSyntax expression, SyntaxKind kind)
    {
        var binary = Assert.IsInstanceOfType<BinaryExpressionSyntax>(expression);
        Assert.AreEqual(kind, binary.Kind);
        return binary;
    }

    private static void AssertIsIdentifier(ExpressionSyntax expression, string name)
    {
        var identifierName = Assert.IsInstanceOfType<IdentifierNameSyntax>(expression);
        Assert.AreEqual(name, identifierName.Identifier.Text);
    }
}

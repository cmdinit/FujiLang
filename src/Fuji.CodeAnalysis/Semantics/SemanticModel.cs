namespace Fuji.CodeAnalysis.Semantics;

using Fuji.CodeAnalysis.Symbols;
using Fuji.CodeAnalysis.Syntax;


public class SemanticModel
{
    private readonly SyntaxTree _syntaxTree;
    private readonly SourceCompilationSymbolTable _symbolTable;
    private readonly Dictionary<ExpressionSyntax, ExpressionBinding> _expressionBindings = new();

    internal SemanticModel(SyntaxTree syntaxTree, SourceCompilationSymbolTable symbolTable)
    {
        _syntaxTree = syntaxTree;
        _symbolTable = symbolTable;
    }

    public SyntaxTree SyntaxTree => _syntaxTree;

    public IReadOnlyCollection<INamedTypeSymbol> NamedTypes => _symbolTable.NamedTypes;
    public IReadOnlyList<IMethodSymbol> Methods => _symbolTable.Methods;

    public ISymbol? GetDeclaredSymbol(SyntaxNode node)
    {
        return _symbolTable.GetDeclaredSymbol(_syntaxTree, node);
    }

    public IEnumerable<ISymbol> GetDeclaredSymbols()
    {
        return _symbolTable.GetDeclaredSymbols(_syntaxTree);
    }

    public ISymbol? GetSymbolInfo(ExpressionSyntax expression)
    {
        return GetOrBindExpression(expression).Symbol;
    }

    public ITypeSymbol GetTypeInfo(ExpressionSyntax expression)
    {
        return GetOrBindExpression(expression).Type;
    }

    public ISymbol? GetSymbolInfo(TypeSyntax typeSyntax)
    {
        return ResolveType(typeSyntax, containingSymbol: null);
    }

    public ITypeSymbol GetTypeInfo(TypeSyntax typeSyntax)
    {
        return ResolveType(typeSyntax, containingSymbol: null);
    }

    private ExpressionBinding GetOrBindExpression(ExpressionSyntax expression)
    {
        if (_expressionBindings.TryGetValue(expression, out var existing))
        {
            return existing;
        }

        var containingMethod = TryGetContainingMethod(expression, out var method) ? method : null;
        var bound = BindExpression(expression, containingMethod);
        _expressionBindings.Add(expression, bound);
        return bound;
    }

    private ExpressionBinding BindExpression(ExpressionSyntax expression, SourceMethodSymbol? containingMethod)
    {
        if (expression is IdentifierNameSyntax identifier)
        {
            return BindIdentifier(identifier, containingMethod);
        }

        if (expression is LiteralExpressionSyntax literal)
        {
            return BindLiteral(literal);
        }

        if (expression is ParenthesisExpressionSyntax parenthesized)
        {
            return BindExpression(parenthesized.Expression, containingMethod);
        }

        if (expression is PrefixUnaryExpressionSyntax prefix)
        {
            var operand = BindExpression(prefix.Operand, containingMethod);
            if (prefix.Kind == SyntaxKind.LogicalNotExpression)
            {
                return new ExpressionBinding(Symbol: null, Type: GetRequiredBuiltinType(SyntaxKind.BoolKeyword));
            }

            return new ExpressionBinding(Symbol: null, Type: operand.Type);
        }

        if (expression is BinaryExpressionSyntax binary)
        {
            var left = BindExpression(binary.Left, containingMethod);
            _ = BindExpression(binary.Right, containingMethod);

            if (IsBooleanResultOperator(binary.Kind))
            {
                return new ExpressionBinding(Symbol: null, Type: GetRequiredBuiltinType(SyntaxKind.BoolKeyword));
            }

            return new ExpressionBinding(Symbol: null, Type: left.Type);
        }

        if (expression is MemberAccessExpressionSyntax memberAccess)
        {
            var receiver = BindExpression(memberAccess.Expression, containingMethod);
            if (receiver.Type is INamedTypeSymbol namedType)
            {
                var field = namedType.Fields.FirstOrDefault(f => string.Equals(f.Name, memberAccess.IdentifierToken.Text, StringComparison.Ordinal));
                if (field != null)
                {
                    return new ExpressionBinding(field, field.Type);
                }

                var missingMemberType = CreateErrorType(
                    memberAccess.IdentifierToken.Text,
                    memberAccess,
                    $"Type '{namedType.Name}' does not contain a field named '{memberAccess.IdentifierToken.Text}'.",
                    receiver.Symbol ?? containingMethod);

                return new ExpressionBinding(Symbol: null, Type: missingMemberType);
            }

            var notStructType = CreateErrorType(
                receiver.Type.Name,
                memberAccess,
                $"Cannot access member '{memberAccess.IdentifierToken.Text}' on non-struct type '{receiver.Type.Name}'.",
                receiver.Symbol ?? containingMethod);

            return new ExpressionBinding(Symbol: null, Type: notStructType);
        }

        if (expression is InvocationExpressionSyntax invocation)
        {
            var target = BindExpression(invocation.Expression, containingMethod);

            for (var i = 0; i < invocation.Arguments.SlotCount; i++)
            {
                if (invocation.Arguments.GetSlot(i) is ArgumentSyntax argument)
                {
                    _ = BindExpression(argument.Expression, containingMethod);
                }
            }

            if (target.Symbol is IMethodSymbol method)
            {
                return new ExpressionBinding(method, method.ReturnType);
            }

            var notInvocable = CreateErrorType(
                target.Type.Name,
                invocation,
                $"Expression of type '{target.Type.Name}' is not invocable.",
                target.Symbol ?? containingMethod);

            return new ExpressionBinding(Symbol: null, Type: notInvocable);
        }

        return new ExpressionBinding(
            Symbol: null,
            Type: CreateErrorType("<expression>", expression, "Unsupported expression syntax.", containingMethod));
    }

    private ExpressionBinding BindIdentifier(IdentifierNameSyntax identifier, SourceMethodSymbol? containingMethod)
    {
        if (containingMethod != null)
        {
            var parameter = containingMethod.Parameters.FirstOrDefault(p => string.Equals(p.Name, identifier.Identifier.Text, StringComparison.Ordinal));
            if (parameter != null)
            {
                return new ExpressionBinding(parameter, parameter.Type);
            }
        }

        if (_symbolTable.TryLookupNamedType(identifier.Identifier.Text, out var namedType))
        {
            return new ExpressionBinding(namedType, namedType);
        }

        if (_symbolTable.TryLookupMethod(identifier.Identifier.Text, out var method))
        {
            return new ExpressionBinding(method, method.ReturnType);
        }

        var missing = CreateErrorType(
            identifier.Identifier.Text,
            identifier,
            $"Unknown identifier '{identifier.Identifier.Text}'.",
            containingMethod);

        return new ExpressionBinding(Symbol: null, Type: missing);
    }

    private static bool IsBooleanResultOperator(SyntaxKind kind)
    {
        return kind == SyntaxKind.EqualsExpression
            || kind == SyntaxKind.NotEqualsExpression
            || kind == SyntaxKind.LessThanExpression
            || kind == SyntaxKind.LessThanOrEqualsExpression
            || kind == SyntaxKind.GreaterThanExpression
            || kind == SyntaxKind.GreaterThanOrEqualsExpression
            || kind == SyntaxKind.LogicalAndExpression
            || kind == SyntaxKind.LogicalOrExpression;
    }

    private ExpressionBinding BindLiteral(LiteralExpressionSyntax literal)
    {
        if (literal.Kind == SyntaxKind.StringLiteralExpression)
        {
            return new ExpressionBinding(Symbol: null, Type: GetRequiredBuiltinType(SyntaxKind.StringKeyword));
        }

        if (literal.Kind == SyntaxKind.CharLiteralExpression)
        {
            return new ExpressionBinding(Symbol: null, Type: GetRequiredBuiltinType(SyntaxKind.CharKeyword));
        }

        if (literal.Kind == SyntaxKind.TrueLiteralExpression || literal.Kind == SyntaxKind.FalseLiteralExpression)
        {
            return new ExpressionBinding(Symbol: null, Type: GetRequiredBuiltinType(SyntaxKind.BoolKeyword));
        }

        if (literal.Kind == SyntaxKind.NumberLiteralExpression)
        {
            return new ExpressionBinding(Symbol: null, Type: GetRequiredBuiltinType(SyntaxKind.IntKeyword));
        }

        if (literal.Kind == SyntaxKind.NullLiteralExpression)
        {
            var nullType = CreateErrorType("null", literal, "Null literal type inference is not implemented yet.");
            return new ExpressionBinding(Symbol: null, Type: nullType);
        }

        return new ExpressionBinding(
            Symbol: null,
            Type: CreateErrorType("<literal>", literal, "Unknown literal expression kind."));
    }

    private ITypeSymbol ResolveType(TypeSyntax syntax, ISymbol? containingSymbol)
    {
        if (syntax is PredefinedTypeSyntax predefined)
        {
            if (BuiltinTypeSymbol.TryGetByPredefinedKeyword(predefined.Keyword.Kind, out var builtin))
            {
                return builtin;
            }

            return CreateErrorType(predefined.Keyword.Text, predefined, $"Unsupported predefined type '{predefined.Keyword.Text}'.", containingSymbol);
        }

        if (syntax is IdentifierNameSyntax identifier)
        {
            if (_symbolTable.TryLookupNamedType(identifier.Identifier.Text, out var namedType))
            {
                return namedType;
            }

            return CreateErrorType(identifier.Identifier.Text, identifier, $"Unknown type '{identifier.Identifier.Text}'.", containingSymbol);
        }

        if (syntax is QualifiedNameSyntax qualifiedName)
        {
            var name = GetQualifiedNameText(qualifiedName);
            if (_symbolTable.TryLookupNamedType(name, out var namedType))
            {
                return namedType;
            }

            return CreateErrorType(name, qualifiedName, $"Unknown type '{name}'.", containingSymbol);
        }

        return CreateErrorType("<type>", syntax, "Unsupported type syntax.", containingSymbol);
    }

    private static string GetQualifiedNameText(QualifiedNameSyntax qualifiedName)
    {
        if (qualifiedName.Left is QualifiedNameSyntax nested)
        {
            return $"{GetQualifiedNameText(nested)}.{qualifiedName.Right.Text}";
        }

        if (qualifiedName.Left is IdentifierNameSyntax identifier)
        {
            return $"{identifier.Identifier.Text}.{qualifiedName.Right.Text}";
        }

        if (qualifiedName.Left is PredefinedTypeSyntax predefined)
        {
            return $"{predefined.Keyword.Text}.{qualifiedName.Right.Text}";
        }

        return qualifiedName.Right.Text;
    }

    private static ITypeSymbol CreateErrorType(string name, SyntaxNode syntax, string message, ISymbol? containingSymbol = null)
    {
        return new SourceErrorTypeSymbol(name, containingSymbol, syntax, message);
    }

    private static ITypeSymbol GetRequiredBuiltinType(SyntaxKind keywordKind)
    {
        if (BuiltinTypeSymbol.TryGetByPredefinedKeyword(keywordKind, out var builtin))
        {
            return builtin;
        }

        throw new InvalidOperationException($"Missing builtin type mapping for syntax kind '{keywordKind}'.");
    }

    private bool TryGetContainingMethod(ExpressionSyntax target, out SourceMethodSymbol? method)
    {
        method = null;
        if (!TryFindContainingFunctionDeclaration(_syntaxTree.Root, target, currentFunction: null, out var containingFunction))
        {
            return false;
        }

        if (containingFunction == null)
        {
            return false;
        }

        method = _symbolTable.GetDeclaredSymbol(_syntaxTree, containingFunction) as SourceMethodSymbol;
        return method != null;
    }

    private static bool TryFindContainingFunctionDeclaration(
        SyntaxNode node,
        ExpressionSyntax target,
        FunctionDeclarationSyntax? currentFunction,
        out FunctionDeclarationSyntax? containingFunction)
    {
        if (ReferenceEquals(node, target))
        {
            containingFunction = currentFunction;
            return true;
        }

        var nextFunction = node as FunctionDeclarationSyntax ?? currentFunction;
        for (var i = 0; i < node.SlotCount; i++)
        {
            var child = node.GetSlot(i);
            if (child == null)
            {
                continue;
            }

            if (TryFindContainingFunctionDeclaration(child, target, nextFunction, out containingFunction))
            {
                return true;
            }
        }

        containingFunction = null;
        return false;
    }

    private sealed record ExpressionBinding(ISymbol? Symbol, ITypeSymbol Type);
}


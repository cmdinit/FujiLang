using Fuji.CodeAnalysis.Symbols;
using Fuji.CodeAnalysis.Syntax;

namespace Fuji.CodeAnalysis.Semantics;

internal sealed class SourceCompilationSymbolTable
{
    private readonly Dictionary<SyntaxTree, Dictionary<SyntaxNode, ISymbol>> _symbolsByTree;
    private readonly Dictionary<string, SourceNamedTypeSymbol> _namedTypesByName;
    private readonly Dictionary<string, List<SourceMethodSymbol>> _methodsByName;

    private SourceCompilationSymbolTable(
        Dictionary<string, SourceNamedTypeSymbol> namedTypes,
        List<SourceMethodSymbol> methods,
        Dictionary<SyntaxTree, Dictionary<SyntaxNode, ISymbol>> symbolsByTree)
    {
        _namedTypesByName = namedTypes;
        _methodsByName = methods
            .GroupBy(m => m.Name, StringComparer.Ordinal)
            .ToDictionary(g => g.Key, g => g.ToList(), StringComparer.Ordinal);
        NamedTypes = _namedTypesByName.Values;
        Methods = methods;
        _symbolsByTree = symbolsByTree;
    }

    public IReadOnlyCollection<SourceNamedTypeSymbol> NamedTypes { get; }
    public IReadOnlyList<SourceMethodSymbol> Methods { get; }

    public IEnumerable<ISymbol> GetDeclaredSymbols(SyntaxTree syntaxTree)
    {
        if (_symbolsByTree.TryGetValue(syntaxTree, out var map))
        {
            return map.Values;
        }

        return [];
    }

    public ISymbol? GetDeclaredSymbol(SyntaxTree syntaxTree, SyntaxNode node)
    {
        if (_symbolsByTree.TryGetValue(syntaxTree, out var map) && map.TryGetValue(node, out var symbol))
        {
            return symbol;
        }

        return null;
    }

    public bool TryLookupNamedType(string name, out SourceNamedTypeSymbol symbol)
    {
        return _namedTypesByName.TryGetValue(name, out symbol!);
    }

    public bool TryLookupMethod(string name, out SourceMethodSymbol method)
    {
        if (_methodsByName.TryGetValue(name, out var methods) && methods.Count > 0)
        {
            method = methods[0];
            return true;
        }

        method = null!;
        return false;
    }

    public static SourceCompilationSymbolTable Build(IEnumerable<SyntaxTree> syntaxTrees)
    {
        var typeByName = new Dictionary<string, SourceNamedTypeSymbol>(StringComparer.Ordinal);
        var methods = new List<SourceMethodSymbol>();
        var symbolsByTree = new Dictionary<SyntaxTree, Dictionary<SyntaxNode, ISymbol>>();

        foreach (var tree in syntaxTrees)
        {
            var map = GetOrCreateTreeMap(symbolsByTree, tree);
            CollectNamedTypeSymbols(tree.Root, typeByName, map);
        }

        foreach (var tree in syntaxTrees)
        {
            var map = GetOrCreateTreeMap(symbolsByTree, tree);
            PopulateMembersAndMethods(tree.Root, typeByName, methods, map);
        }

        return new SourceCompilationSymbolTable(typeByName, methods, symbolsByTree);
    }

    private static Dictionary<SyntaxNode, ISymbol> GetOrCreateTreeMap(
        Dictionary<SyntaxTree, Dictionary<SyntaxNode, ISymbol>> symbolsByTree,
        SyntaxTree tree)
    {
        if (symbolsByTree.TryGetValue(tree, out var existing))
        {
            return existing;
        }

        var created = new Dictionary<SyntaxNode, ISymbol>();
        symbolsByTree[tree] = created;
        return created;
    }

    private static void CollectNamedTypeSymbols(
        CompilationUnitSyntax root,
        Dictionary<string, SourceNamedTypeSymbol> typeByName,
        Dictionary<SyntaxNode, ISymbol> map)
    {
        for (var i = 0; i < root.Declarations.SlotCount; i++)
        {
            if (root.Declarations.GetSlot(i) is not StructDeclarationSyntax structDecl)
            {
                continue;
            }

            var name = structDecl.Identifier.Text;
            if (!typeByName.TryGetValue(name, out var typeSymbol))
            {
                typeSymbol = new SourceNamedTypeSymbol(name, structDecl);
                typeByName.Add(name, typeSymbol);
            }

            map[structDecl] = typeSymbol;
        }
    }

    private static void PopulateMembersAndMethods(
        CompilationUnitSyntax root,
        Dictionary<string, SourceNamedTypeSymbol> typeByName,
        List<SourceMethodSymbol> methods,
        Dictionary<SyntaxNode, ISymbol> map)
    {
        for (var i = 0; i < root.Declarations.SlotCount; i++)
        {
            var declaration = root.Declarations.GetSlot(i);

            if (declaration is StructDeclarationSyntax structDecl)
            {
                var containingType = typeByName[structDecl.Identifier.Text];
                BindStructFields(structDecl, containingType, typeByName, map);
                continue;
            }

            if (declaration is FunctionDeclarationSyntax functionDecl)
            {
                var method = new SourceMethodSymbol(functionDecl.Identifier.Text, BuiltinTypeSymbol.Void, functionDecl);
                methods.Add(method);
                map[functionDecl] = method;
                BindMethodParameters(functionDecl, method, typeByName, map);
            }
        }
    }

    private static void BindStructFields(
        StructDeclarationSyntax structDecl,
        SourceNamedTypeSymbol containingType,
        Dictionary<string, SourceNamedTypeSymbol> typeByName,
        Dictionary<SyntaxNode, ISymbol> map)
    {
        for (var i = 0; i < structDecl.Declarations.SlotCount; i++)
        {
            if (structDecl.Declarations.GetSlot(i) is not FieldDeclarationSyntax fieldDecl)
            {
                continue;
            }

            var fieldType = ResolveTypeSymbol(fieldDecl.Type, containingType, typeByName);
            var field = new SourceFieldSymbol(fieldDecl.Identifier.Text, fieldType, containingType, fieldDecl);
            containingType.AddField(field);
            map[fieldDecl] = field;
        }
    }

    private static void BindMethodParameters(
        FunctionDeclarationSyntax functionDecl,
        SourceMethodSymbol containingMethod,
        Dictionary<string, SourceNamedTypeSymbol> typeByName,
        Dictionary<SyntaxNode, ISymbol> map)
    {
        var ordinal = 0;
        for (var i = 0; i < functionDecl.Parameters.SlotCount; i++)
        {
            if (functionDecl.Parameters.GetSlot(i) is not ParameterSyntax parameterDecl)
            {
                continue;
            }

            var parameterType = ResolveTypeSymbol(parameterDecl.Type, containingMethod, typeByName);
            var parameter = new SourceParameterSymbol(parameterDecl.Identifier.Text, ordinal, parameterType, containingMethod, parameterDecl);
            containingMethod.AddParameter(parameter);
            map[parameterDecl] = parameter;
            ordinal++;
        }
    }

    private static ITypeSymbol ResolveTypeSymbol(
        TypeSyntax syntax,
        ISymbol? containingSymbol,
        Dictionary<string, SourceNamedTypeSymbol> typeByName)
    {
        if (syntax is PredefinedTypeSyntax predefinedType)
        {
            if (BuiltinTypeSymbol.TryGetByPredefinedKeyword(predefinedType.Keyword.Kind, out var builtin))
            {
                return builtin;
            }

            return new SourceErrorTypeSymbol(
                predefinedType.Keyword.Text,
                containingSymbol,
                predefinedType,
                $"Unsupported predefined type '{predefinedType.Keyword.Text}'.");
        }

        if (syntax is IdentifierNameSyntax identifierName)
        {
            var name = identifierName.Identifier.Text;
            if (typeByName.TryGetValue(name, out var namedType))
            {
                return namedType;
            }

            return new SourceErrorTypeSymbol(name, containingSymbol, identifierName, $"Unknown type '{name}'.");
        }

        if (syntax is QualifiedNameSyntax qualifiedName)
        {
            var name = GetQualifiedNameText(qualifiedName);
            if (typeByName.TryGetValue(name, out var namedType))
            {
                return namedType;
            }

            return new SourceErrorTypeSymbol(name, containingSymbol, qualifiedName, $"Unknown type '{name}'.");
        }

        return new SourceErrorTypeSymbol("<unknown>", containingSymbol, syntax, "Unsupported type syntax.");
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
}

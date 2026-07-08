using Fuji.CodeAnalysis.Syntax;

namespace Fuji.CodeAnalysis.Symbols;

public abstract class Symbol : ISymbol
{
    protected Symbol(string name, SymbolKind kind, ISymbol? containingSymbol, SyntaxNode? declaringSyntax)
    {
        Name = name;
        Kind = kind;
        ContainingSymbol = containingSymbol;
        DeclaringSyntax = declaringSyntax;
    }

    public string Name { get; }
    public SymbolKind Kind { get; }
    public ISymbol? ContainingSymbol { get; }
    public SyntaxNode? DeclaringSyntax { get; }
}

public abstract class TypeSymbol : Symbol, ITypeSymbol
{
    protected TypeSymbol(
        string name,
        SymbolKind kind,
        TypeKind typeKind,
        ISymbol? containingSymbol,
        SyntaxNode? declaringSyntax)
        : base(name, kind, containingSymbol, declaringSyntax)
    {
        TypeKind = typeKind;
    }

    public TypeKind TypeKind { get; }
}

public sealed class BuiltinTypeSymbol : TypeSymbol
{
    private static readonly Dictionary<SyntaxKind, BuiltinTypeSymbol> s_predefinedByKeyword = new()
    {
        [SyntaxKind.IntKeyword] = new BuiltinTypeSymbol("int"),
        [SyntaxKind.FloatKeyword] = new BuiltinTypeSymbol("float"),
        [SyntaxKind.BoolKeyword] = new BuiltinTypeSymbol("bool"),
        [SyntaxKind.StringKeyword] = new BuiltinTypeSymbol("string"),
        [SyntaxKind.CharKeyword] = new BuiltinTypeSymbol("char"),
        [SyntaxKind.DoubleKeyword] = new BuiltinTypeSymbol("double"),
        [SyntaxKind.Int8Keyword] = new BuiltinTypeSymbol("int8"),
        [SyntaxKind.Int16Keyword] = new BuiltinTypeSymbol("int16"),
        [SyntaxKind.Int32Keyword] = new BuiltinTypeSymbol("int32"),
        [SyntaxKind.Int64Keyword] = new BuiltinTypeSymbol("int64"),
        [SyntaxKind.UIntKeyword] = new BuiltinTypeSymbol("uint"),
        [SyntaxKind.UInt8Keyword] = new BuiltinTypeSymbol("uint8"),
        [SyntaxKind.UInt16Keyword] = new BuiltinTypeSymbol("uint16"),
        [SyntaxKind.UInt32Keyword] = new BuiltinTypeSymbol("uint32"),
        [SyntaxKind.UInt64Keyword] = new BuiltinTypeSymbol("uint64"),
    };

    public static BuiltinTypeSymbol Void { get; } = new("void");

    private BuiltinTypeSymbol(string name)
        : base(name, SymbolKind.BuiltinType, TypeKind.Builtin, containingSymbol: null, declaringSyntax: null)
    {
    }

    public static bool TryGetByPredefinedKeyword(SyntaxKind kind, out BuiltinTypeSymbol type)
    {
        return s_predefinedByKeyword.TryGetValue(kind, out type!);
    }
}

public sealed class SourceNamedTypeSymbol : TypeSymbol, INamedTypeSymbol
{
    private readonly List<IFieldSymbol> _fields = [];

    public SourceNamedTypeSymbol(string name, StructDeclarationSyntax declaringSyntax)
        : base(name, SymbolKind.NamedType, TypeKind.Struct, containingSymbol: null, declaringSyntax)
    {
    }

    public IReadOnlyList<IFieldSymbol> Fields => _fields;
    StructDeclarationSyntax INamedTypeSymbol.DeclaringSyntax => (StructDeclarationSyntax)DeclaringSyntax!;

    internal void AddField(SourceFieldSymbol field)
    {
        _fields.Add(field);
    }
}

public sealed class SourceFieldSymbol : Symbol, IFieldSymbol
{
    public SourceFieldSymbol(string name, ITypeSymbol type, SourceNamedTypeSymbol containingType, FieldDeclarationSyntax declaringSyntax)
        : base(name, SymbolKind.Field, containingType, declaringSyntax)
    {
        Type = type;
        ContainingType = containingType;
    }

    public ITypeSymbol Type { get; }
    public INamedTypeSymbol ContainingType { get; }
    FieldDeclarationSyntax IFieldSymbol.DeclaringSyntax => (FieldDeclarationSyntax)DeclaringSyntax!;
}

public sealed class SourceMethodSymbol : Symbol, IMethodSymbol
{
    private readonly List<IParameterSymbol> _parameters = [];

    public SourceMethodSymbol(string name, ITypeSymbol returnType, FunctionDeclarationSyntax declaringSyntax)
        : base(name, SymbolKind.Method, containingSymbol: null, declaringSyntax)
    {
        ReturnType = returnType;
    }

    public ITypeSymbol ReturnType { get; }
    public IReadOnlyList<IParameterSymbol> Parameters => _parameters;
    FunctionDeclarationSyntax IMethodSymbol.DeclaringSyntax => (FunctionDeclarationSyntax)DeclaringSyntax!;

    internal void AddParameter(SourceParameterSymbol parameter)
    {
        _parameters.Add(parameter);
    }
}

public sealed class SourceParameterSymbol : Symbol, IParameterSymbol
{
    public SourceParameterSymbol(string name, int ordinal, ITypeSymbol type, SourceMethodSymbol containingMethod, ParameterSyntax declaringSyntax)
        : base(name, SymbolKind.Parameter, containingMethod, declaringSyntax)
    {
        Ordinal = ordinal;
        Type = type;
        ContainingMethod = containingMethod;
    }

    public int Ordinal { get; }
    public ITypeSymbol Type { get; }
    public IMethodSymbol ContainingMethod { get; }
    ParameterSyntax IParameterSymbol.DeclaringSyntax => (ParameterSyntax)DeclaringSyntax!;
}

public sealed class SourceLocalSymbol : Symbol, ILocalSymbol
{
    public SourceLocalSymbol(string name, int ordinal, ITypeSymbol type, SourceMethodSymbol containingMethod, SyntaxNode? declaringSyntax = null)
        : base(name, SymbolKind.Local, containingMethod, declaringSyntax)
    {
        Ordinal = ordinal;
        Type = type;
        ContainingMethod = containingMethod;
    }

    public int Ordinal { get; }
    public ITypeSymbol Type { get; }
    public IMethodSymbol ContainingMethod { get; }
}

public sealed class SourceErrorTypeSymbol : TypeSymbol, IErrorTypeSymbol
{
    public SourceErrorTypeSymbol(string name, ISymbol? containingSymbol = null, SyntaxNode? declaringSyntax = null, string? diagnosticMessage = null)
        : base(name, SymbolKind.ErrorType, TypeKind.Error, containingSymbol, declaringSyntax)
    {
        DiagnosticMessage = diagnosticMessage;
    }

    public string? DiagnosticMessage { get; }
}

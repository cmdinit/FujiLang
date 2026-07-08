namespace Fuji.CodeAnalysis.Symbols;

using Fuji.CodeAnalysis.Syntax;

public interface ISymbol
{
    string Name { get; }
    SymbolKind Kind { get; }
    ISymbol? ContainingSymbol { get; }
    SyntaxNode? DeclaringSyntax { get; }
}

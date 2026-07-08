namespace Fuji.CodeAnalysis.Symbols;

using Fuji.CodeAnalysis.Syntax;

public interface INamedTypeSymbol : ITypeSymbol
{
    IReadOnlyList<IFieldSymbol> Fields { get; }
    new StructDeclarationSyntax DeclaringSyntax { get; }
}

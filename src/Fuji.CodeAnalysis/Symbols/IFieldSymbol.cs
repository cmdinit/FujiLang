namespace Fuji.CodeAnalysis.Symbols;

using Fuji.CodeAnalysis.Syntax;

public interface IFieldSymbol : ISymbol
{
    ITypeSymbol Type { get; }
    INamedTypeSymbol ContainingType { get; }
    new FieldDeclarationSyntax DeclaringSyntax { get; }
}

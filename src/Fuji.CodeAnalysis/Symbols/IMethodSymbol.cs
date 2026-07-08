namespace Fuji.CodeAnalysis.Symbols;

using Fuji.CodeAnalysis.Syntax;

public interface IMethodSymbol : ISymbol
{
    ITypeSymbol ReturnType { get; }
    IReadOnlyList<IParameterSymbol> Parameters { get; }
    new FunctionDeclarationSyntax DeclaringSyntax { get; }
}

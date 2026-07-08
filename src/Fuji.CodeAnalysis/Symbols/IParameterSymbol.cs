namespace Fuji.CodeAnalysis.Symbols;

using Fuji.CodeAnalysis.Syntax;

public interface IParameterSymbol : ISymbol
{
    int Ordinal { get; }
    ITypeSymbol Type { get; }
    IMethodSymbol ContainingMethod { get; }
    new ParameterSyntax DeclaringSyntax { get; }
}

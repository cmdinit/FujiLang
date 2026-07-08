namespace Fuji.CodeAnalysis.Symbols;

public interface ILocalSymbol : ISymbol
{
    int Ordinal { get; }
    ITypeSymbol Type { get; }
    IMethodSymbol ContainingMethod { get; }
}

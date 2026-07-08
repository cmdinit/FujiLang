namespace Fuji.CodeAnalysis.Symbols;

public interface ITypeSymbol : ISymbol
{
    TypeKind TypeKind { get; }
}

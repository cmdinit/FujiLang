namespace Fuji.CodeAnalysis.Symbols;

public interface IErrorTypeSymbol : ITypeSymbol
{
    string? DiagnosticMessage { get; }
}

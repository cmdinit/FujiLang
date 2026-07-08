using Fuji.CodeAnalysis.Parser;
using Fuji.CodeAnalysis.Text;

namespace Fuji.CodeAnalysis.Syntax;

public abstract class SyntaxTree
{
    public abstract SourceText SourceText { get; }
    public abstract CompilationUnitSyntax Root { get; }
}

public class ParsedSyntaxTree : SyntaxTree
{
    private readonly CompilationUnitSyntax _root;
    public ParsedSyntaxTree(SourceText sourceText)
    {
        SourceText = sourceText;
        _root = new LanguageParser(sourceText).ParseCompilationUnit();
    }

    public override SourceText SourceText { get; }
    public override CompilationUnitSyntax Root => _root;
}
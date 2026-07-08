using Fuji.CodeAnalysis.Syntax;

namespace Fuji.CodeAnalysis.Semantics;

public class Compilation
{
    private readonly List<SyntaxTree> _syntaxTrees;
    private readonly Dictionary<SyntaxTree, SemanticModel> _semanticModels = new();
    private readonly SourceCompilationSymbolTable _symbolTable;

    public static Compilation Create(List<SyntaxTree> syntaxTrees)
    {
        return new Compilation(syntaxTrees);
    }

    private Compilation(List<SyntaxTree> syntaxTrees)
    {
        _syntaxTrees = syntaxTrees;
        _symbolTable = SourceCompilationSymbolTable.Build(_syntaxTrees);
    }

    public SemanticModel GetSemanticModel(SyntaxTree syntaxTree)
    {
        if (!_syntaxTrees.Contains(syntaxTree))
        {
            throw new ArgumentException("Syntax tree does not belong to this compilation.");
        }

        if (_semanticModels.TryGetValue(syntaxTree, out var existing))
        {
            return existing;
        }

        var created = new SemanticModel(syntaxTree, _symbolTable);
        _semanticModels.Add(syntaxTree, created);
        return created;
    }
}

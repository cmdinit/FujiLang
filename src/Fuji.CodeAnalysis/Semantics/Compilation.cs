using Fuji.CodeAnalysis.Syntax;

namespace Fuji.CodeAnalysis.Semantics;

public class Compilation
{
    private readonly List<SyntaxTree> _syntaxTrees;

    public static Compilation Create(List<SyntaxTree> syntaxTrees)
    {
        return new Compilation(syntaxTrees);
    }

    private Compilation(List<SyntaxTree> syntaxTrees)
    {
        _syntaxTrees = syntaxTrees;
    }

    public SemanticModel GetSemanticModel(SyntaxTree syntaxTree)
    {
        if (!_syntaxTrees.Contains(syntaxTree))
        {
            throw new ArgumentException("Syntax tree does not belong to this compilation.");
        }

        return new SemanticModel();
    }
}

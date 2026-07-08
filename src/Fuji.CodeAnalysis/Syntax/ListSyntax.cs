namespace Fuji.CodeAnalysis.Syntax;

#region ListSyntax

public abstract class ListSyntax : SyntaxNode
{
    protected SyntaxNode[] _children = [];

    protected ListSyntax(SyntaxKind kind, SyntaxNode[] children) : base(kind)
    {
        _children = children;
        SlotCount = _children.Length;
        foreach (var child in _children)
        {
            AdjustWidth(child);
        }
    }

    public override SyntaxNode? GetSlot(int index)
    {
        if (index < 0 || index >= _children.Length)
            return null;

        return _children[index];
    }

    public override bool IsList => true;
}

public class ArgumentListSyntax : ListSyntax
{
    public static readonly ArgumentListSyntax Empty = new([]);

    public static SeparatedListSyntaxBuilder<ArgumentSyntax, ArgumentListSyntax> GetBuilder()
    {
        return new SeparatedListSyntaxBuilder<ArgumentSyntax, ArgumentListSyntax>(Create);
    }

    private static ArgumentListSyntax Create(SyntaxNode[] arguments)
    {
        return new ArgumentListSyntax(arguments);
    }

    private ArgumentListSyntax(SyntaxNode[] arguments)
        : base(SyntaxKind.ArgumentList, arguments)
    {
    }
}

public class ParameterListSyntax : ListSyntax
{
    public static readonly ParameterListSyntax Empty = new([]);

    public static SeparatedListSyntaxBuilder<ParameterSyntax, ParameterListSyntax> GetBuilder()
    {
        return new SeparatedListSyntaxBuilder<ParameterSyntax, ParameterListSyntax>(Create);
    }

    private static ParameterListSyntax Create(SyntaxNode[] parameters)
    {
        return new ParameterListSyntax(parameters);
    }

    private ParameterListSyntax(SyntaxNode[] parameters)
        : base(SyntaxKind.ParameterList, parameters)
    {
    }
}


public class StatementListSyntax : ListSyntax
{
    public static readonly StatementListSyntax Empty = new([]);

    public static SyntaxListBuilder<StatementSyntax, StatementListSyntax> GetBuilder()
    {
        return new SyntaxListBuilder<StatementSyntax, StatementListSyntax>(Create);
    }

    private StatementListSyntax(StatementSyntax[] statements)
        : base(SyntaxKind.StatementList, statements)
    {
    }
    private static StatementListSyntax Create(StatementSyntax[] statements)
    {
        return new StatementListSyntax(statements);
    }
}

public class RootDeclarationListSyntax : ListSyntax
{
    public static readonly RootDeclarationListSyntax Empty = new([]);

    public static SyntaxListBuilder<RootDeclarationSyntax, RootDeclarationListSyntax> GetBuilder()
    {
        return new SyntaxListBuilder<RootDeclarationSyntax, RootDeclarationListSyntax>(Create);
    }

    private RootDeclarationListSyntax(RootDeclarationSyntax[] declarations)
        : base(SyntaxKind.RootDeclarationList, declarations)
    {
    }
    private static RootDeclarationListSyntax Create(RootDeclarationSyntax[] declarations)
    {
        return new RootDeclarationListSyntax(declarations);
    }
}

public class MemberDeclarationListSyntax : ListSyntax
{
    public static readonly MemberDeclarationListSyntax Empty = new([]);

    public static SyntaxListBuilder<MemberDeclarationSyntax, MemberDeclarationListSyntax> GetBuilder()
    {
        return new SyntaxListBuilder<MemberDeclarationSyntax, MemberDeclarationListSyntax>(Create);
    }

    private MemberDeclarationListSyntax(MemberDeclarationSyntax[] declarations)
        : base(SyntaxKind.MemberDeclarationList, declarations)
    {
    }
    private static MemberDeclarationListSyntax Create(MemberDeclarationSyntax[] declarations)
    {
        return new MemberDeclarationListSyntax(declarations);
    }
}

#endregion

#region Builders

public class SyntaxListBuilder<TNode, TList> where TNode : SyntaxNode where TList : ListSyntax
{
    private readonly List<TNode> _nodes = [];
    private readonly Func<TNode[], TList> _create;

    public SyntaxListBuilder(Func<TNode[], TList> create)
    {
        _create = create;
    }

    public void Add(TNode node)
    {
        _nodes.Add(node);
    }

    public TList Build()
    {
        return _create(_nodes.ToArray());
    }
}

public class SeparatedListSyntaxBuilder<TNode, TList> where TNode : SyntaxNode where TList : ListSyntax
{
    private readonly List<SyntaxNode> _children = new();
    private readonly Func<SyntaxNode[], TList> _createNode;

    public SeparatedListSyntaxBuilder(Func<SyntaxNode[], TList> createNode)
    {
        _createNode = createNode;
    }

    public void AddNode(TNode node)
    {
        _children.Add(node);
    }

    public void AddSeparator(SyntaxToken separator)
    {
        _children.Add(separator);
    }

    public TList Build()
    {
        return _createNode(_children.ToArray());
    }
}

#endregion
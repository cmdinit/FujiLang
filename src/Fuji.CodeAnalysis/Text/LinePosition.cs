namespace Fuji.CodeAnalysis.Text;

public readonly struct LinePosition
{
    public readonly int Line;
    public readonly int Character;

    public LinePosition(int line, int character)
    {
        Line = line;
        Character = character;
    }
}

namespace Fuji.CodeAnalysis.Text;

public readonly struct LinePositionSpan
{
    public readonly LinePosition Start;
    public readonly LinePosition End;

    public LinePositionSpan(LinePosition start, LinePosition end)
    {
        Start = start;
        End = end;
    }
}
namespace Fuji.CodeAnalysis.Text;

public readonly struct TextSpan
{
    public readonly int Start;
    public readonly int Length;
    public readonly int End;

    public TextSpan(int start, int length)
    {
        Start = start;
        Length = length;
        End = Start + Length;
    }

    public bool Contains(int position) => position >= Start && position < End;
    public bool Overlaps(TextSpan other) => Start < other.End && End > other.Start;
    public bool IsEmpty => Length == 0;
}

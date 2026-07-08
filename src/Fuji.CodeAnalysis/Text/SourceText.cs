namespace Fuji.CodeAnalysis.Text;

public sealed class SourceText
{
    public readonly string Text;
    public readonly int Length;
    private int _index;
    private int _start;

    public SourceText(string text)
    {
        Text = text;
        Length = text.Length;
        _index = 0;
        _start = 0;
    }

    public void ResetStart()
    {
        _start = _index;
    }

    public char Peek(int offset)
    {
        var index = _index + offset;
        if (index >= Length)
            return '\0';
        return Text[index];
    }

    public void Eat(int count = 1)
    {
        _index += count;
    }

    public string GetText() => Text[_start.._index];
    public char Current => Peek(0);
    public char LookAhead => Peek(1);
    public bool IsAtEnd => _index >= Length;
    public int Position => _index;
}

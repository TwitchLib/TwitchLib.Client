using System;

namespace TwitchLib.Client.Models.Extensions;

internal ref struct SpanSliceEnumerator
{
    ReadOnlySpan<char> _span;
    readonly char _char;

    public SpanSliceEnumerator(ReadOnlySpan<char> span, char @char)
    {
        _span = span;
        _char = @char;
    }

    public SpanSliceEnumerator(string str, char @char) : this(str.AsSpan(), @char)
    { }

    public ReadOnlySpan<char> Current { get; private set; }

    public SpanSliceEnumerator GetEnumerator() => this;

    public bool MoveNext()
    {
        if (_span.IsEmpty)
            return false;
        var index = _span.IndexOf(_char);

        if (index == -1)
        {
            Current = _span;
            _span = default;
        }
        else
        {
            Current = _span.Slice(0, index);
            _span = _span.Slice(index + 1);
        }
        return true;
    }
}

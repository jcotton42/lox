using System;

namespace Lox.Analyzers;

public static class SpanExtensions
{
    public static DoubleReadOnlySpan<T> SplitAround<T>(this ReadOnlySpan<T> span, T delimiter) where T : IEquatable<T>
        => span.IndexOf(delimiter) switch
        {
            < 0 => new DoubleReadOnlySpan<T> { First = span, Second = [] },
            var index => new DoubleReadOnlySpan<T> { First = span[..index], Second = span[(index + 1)..] },
        };

    public readonly ref struct DoubleReadOnlySpan<T>
    {
        public required ReadOnlySpan<T> First { get; init; }
        public required ReadOnlySpan<T> Second { get; init; }

        public void Deconstruct(out ReadOnlySpan<T> first, out ReadOnlySpan<T> second)
        {
            first = First;
            second = Second;
        }
    }
}

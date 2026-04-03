using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Lox.Generator;

public readonly struct EquatableImmutableArray<T> : IEquatable<EquatableImmutableArray<T>>
{
    public static EquatableImmutableArray<T> Empty { get; } = new() { Items = ImmutableArray<T>.Empty };

    public required ImmutableArray<T> Items { get; init; }

    public override bool Equals(object? obj) => obj is EquatableImmutableArray<T> other && Equals(other);

    public bool Equals(EquatableImmutableArray<T> other)
    {
        if (Items.IsDefaultOrEmpty && other.Items.IsDefaultOrEmpty) return true;
        if (Items.Length != other.Items.Length) return false;

        for (var i = 0; i < Items.Length; i++)
        {
            if (!EqualityComparer<T>.Default.Equals(Items[i], other.Items[i]))
            {
                return false;
            }
        }

        return true;
    }

    public override int GetHashCode()
    {
        if (Items.IsDefaultOrEmpty) return 0;

        var hash = new HashCode();
        foreach (var item in Items)
        {
            hash.Add(item);
        }

        return hash.ToHashCode();
    }
}

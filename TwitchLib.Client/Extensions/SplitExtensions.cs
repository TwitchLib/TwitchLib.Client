// from: https://gist.github.com/neon-sunset/df6fb9fe6bb71f11c2b47fbeae55e6da
// proposal: https://github.com/dotnet/runtime/issues/75317

using System;
using System.Runtime.CompilerServices;

namespace TwitchLib.Client.Extensions
{
    internal static class SplitExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ReadOnlySplitPair<T> SplitFirst<T>(this ReadOnlySpan<T> source, T separator)
            where T : IEquatable<T>
        {
            var separatorIndex = source.IndexOf(separator);

            return separatorIndex > -1
                ? new(source.Slice(0, separatorIndex), source.Slice(separatorIndex + 1))
                : new(source, default);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ReadOnlySplitPair<T> SplitLast<T>(this ReadOnlySpan<T> source, T separator)
            where T : IEquatable<T>
        {
            var separatorIndex = source.LastIndexOf(separator);

            return separatorIndex > -1
                ? new(source.Slice(0, separatorIndex), source.Slice(separatorIndex + 1))
                : new(source, default);
        }

        internal readonly ref struct ReadOnlySplitPair<T>
        {
            public readonly ReadOnlySpan<T> Left;

            public readonly ReadOnlySpan<T> Right;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ReadOnlySplitPair(ReadOnlySpan<T> left, ReadOnlySpan<T> right)
            {
                Left = left;
                Right = right;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Deconstruct(out ReadOnlySpan<T> left, out ReadOnlySpan<T> right)
            {
                left = Left;
                right = Right;
            }
        }
    }
}
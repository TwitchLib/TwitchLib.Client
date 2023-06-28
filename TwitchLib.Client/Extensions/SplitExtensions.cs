// proposal: https://github.com/dotnet/runtime/issues/75317
using System;
using System.Runtime.CompilerServices;

namespace TwitchLib.Client.Extensions
{
    internal static class SplitExtensions
    {
        /// <summary>
        /// Splits the span into two parts at the first occurrence of a separator.
        /// If the separator is not found, Segment will be the entire span and Remainder will be empty.
        /// </summary>
        /// <typeparam name="T">Span element type</typeparam>
        /// <param name="source">Source span to split</param>
        /// <param name="separator">Separator value</param>
        /// <returns>A split pair of Segment and Remainder, deconstructible with tuple pattern.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ReadOnlySplitPair<T> SplitFirst<T>(this ReadOnlySpan<T> source, T separator)
            where T : IEquatable<T>
        {
            var separatorIndex = source.IndexOf(separator);

            return separatorIndex > -1
                ? new(source.Slice(0, separatorIndex), source.Slice(separatorIndex + 1))
                : new(source, default);
        }

        /// <summary>
        /// Splits the span into two parts at the first occurrence of a separator.
        /// If the separator is not found, Segment will be the entire span and Remainder will be empty.
        /// </summary>
        /// <typeparam name="T">Span element type</typeparam>
        /// <param name="source">Source span to split</param>
        /// <param name="separator">Separator value</param>
        /// <returns>A split pair of Segment and Remainder, deconstructible with tuple pattern.</returns>
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
            public readonly ReadOnlySpan<T> Segment;

            public readonly ReadOnlySpan<T> Remainder;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ReadOnlySplitPair(ReadOnlySpan<T> segment, ReadOnlySpan<T> remainder)
            {
                Segment = segment;
                Remainder = remainder;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Deconstruct(out ReadOnlySpan<T> segment, out ReadOnlySpan<T> remainder)
            {
                segment = Segment;
                remainder = Remainder;
            }
        }
    }
}
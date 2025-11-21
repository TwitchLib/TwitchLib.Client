// proposal: https://github.com/dotnet/runtime/issues/75317
using System.Runtime.CompilerServices;

namespace TwitchLib.Client.Extensions
{
    internal static class SplitExtensions
    {
        /// <summary>
        /// Splits the string into two parts at the first occurrence of a separator.
        /// If the separator is not found, Segment will be the entire span and Remainder will be empty.
        /// </summary>
        /// <param name="source">Source span to split</param>
        /// <param name="separator">Separator value</param>
        /// <returns>A split pair of Segment and Remainder, deconstructible with tuple pattern.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ReadOnlySplitPair<char> SplitFirst(this string source, char separator)
        {
            return SplitFirst(source.AsSpan(), separator);
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
        internal static ReadOnlySplitPair<T> SplitFirst<T>(this ReadOnlySpan<T> source, T separator)
            where T : IEquatable<T>
        {
            var separatorIndex = source.IndexOf(separator);

            return separatorIndex >= 0 ? new(source, separatorIndex, 1) : SplitNotFound(source);
        }

        /// <summary>
        /// Splits the string into two parts at the last occurrence of a separator.
        /// If the separator is not found, Segment will be the entire span and Remainder will be empty.
        /// </summary>
        /// <param name="source">Source span to split</param>
        /// <param name="separator">Separator value</param>
        /// <returns>A split pair of Segment and Remainder, deconstructible with tuple pattern.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ReadOnlySplitPair<char> SplitLast(this string source, char separator)
        {
            return SplitLast(source.AsSpan(), separator);
        }

        /// <summary>
        /// Splits the span into two parts at the last occurrence of a separator.
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

            return separatorIndex >= 0 ? new(source, separatorIndex, 1) : SplitNotFound(source);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ReadOnlySplitPair<T> SplitNotFound<T>(this ReadOnlySpan<T> source)
        {
            return new(source, source.Length, 0);
        }

        internal readonly ref struct ReadOnlySplitPair<T>
        {
            private readonly ReadOnlySpan<T> _source;
            private readonly int _offset;
            private readonly int _stride;

            public readonly ReadOnlySpan<T> Segment => _source.Slice(0, _offset);

            public readonly ReadOnlySpan<T> Remainder => _source.Slice(_offset + _stride);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ReadOnlySplitPair(ReadOnlySpan<T> source, int offset, int stride)
            {
                _source = source;
                _offset = offset;
                _stride = stride;
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

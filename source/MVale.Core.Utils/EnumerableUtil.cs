using System;
using System.Collections.Generic;
using System.Linq;

namespace MVale.Core.Utils
{
    /// <summary>
    /// A utility class providing functionality for sequences.
    /// Related: <seealso cref="IEnumerable{T}"/>
    /// </summary>
    public static class EnumerableUtil
    {
        /// <summary>
        /// From <see cref="byte.MinValue"/> to <see cref="byte.MaxValue"/>.
        /// </summary>
        public static IEnumerable<byte> ByteValues
        {
            get
            {
                yield return byte.MinValue;

                for (byte b = byte.MinValue; b < byte.MaxValue;)
                    yield return ++b;
            }
        }

        /// <summary>
        /// From <see cref="sbyte.MinValue"/> to <see cref="sbyte.MaxValue"/>.
        /// </summary>
        [CLSCompliant(false)]
        public static IEnumerable<sbyte> SByteValues
        {
            get
            {
                yield return sbyte.MinValue;

                for (sbyte b = sbyte.MinValue; b < sbyte.MaxValue;)
                    yield return ++b;
            }
        }

        /// <summary>
        /// From <see cref="short.MinValue"/> to <see cref="short.MaxValue"/>.
        /// </summary>
        public static IEnumerable<short> ShortValues
        {
            get
            {
                yield return short.MinValue;

                for (short s = short.MinValue; s < short.MaxValue;)
                    yield return ++s;
            }
        }

        /// <summary>
        /// From <see cref="ushort.MinValue"/> to <see cref="ushort.MaxValue"/>.
        /// </summary>
        [CLSCompliant(false)]
        public static IEnumerable<ushort> UShortValues
        {
            get
            {
                yield return ushort.MinValue;

                for (ushort s = ushort.MinValue; s < ushort.MaxValue;)
                    yield return ++s;
            }
        }

        /// <summary>
        /// From <see cref="char.MinValue"/> to <see cref="char.MaxValue"/>.
        /// </summary>
        public static IEnumerable<char> CharValues
        {
            get
            {
                yield return char.MinValue;

                for (char c = char.MinValue; c < char.MaxValue;)
                    yield return ++c;
            }
        }

        /// <summary>
        /// From <see cref="int.MinValue"/> to <see cref="int.MaxValue"/>.
        /// </summary>
        public static IEnumerable<int> IntValues
        {
            get
            {
                yield return int.MinValue;

                for (int i = int.MinValue; i < int.MaxValue;)
                    yield return ++i;
            }
        }

        /// <summary>
        /// From <see cref="uint.MinValue"/> to <see cref="uint.MaxValue"/>.
        /// </summary>
        [CLSCompliant(false)]
        public static IEnumerable<uint> UIntValues
        {
            get
            {
                yield return uint.MinValue;

                for (uint i = uint.MinValue; i < uint.MaxValue;)
                    yield return ++i;
            }
        }

        /// <summary>
        /// From <see cref="long.MinValue"/> to <see cref="long.MaxValue"/>.
        /// </summary>
        public static IEnumerable<long> LongValues
        {
            get
            {
                yield return long.MinValue;

                for (long l = long.MinValue; l < long.MaxValue;)
                    yield return ++l;
            }
        }

        /// <summary>
        /// From <see cref="ulong.MinValue"/> to <see cref="ulong.MaxValue"/>.
        /// </summary>
        [CLSCompliant(false)]
        public static IEnumerable<ulong> ULongValues
        {
            get
            {
                yield return ulong.MinValue;

                for (ulong l = ulong.MinValue; l < ulong.MaxValue;)
                    yield return ++l;
            }
        }

        /// <summary>
        /// Create an empty sequence.
        /// </summary>
        /// <typeparam name="T">The generic type of the sequence.</typeparam>
        /// <returns>A sequence containing no elements.</returns>
        public static IEnumerable<T> Create<T>()
        {
            yield break;
        }

        /// <summary>
        /// Create a single item sequence.
        /// </summary>
        /// <typeparam name="T">The generic type of the sequence.</typeparam>
        /// <param name="item">The element that will end up in the sequence.</param>
        /// <returns>A sequence containing an element.</returns>
        public static IEnumerable<T> Create<T>(T item)
        {
            yield return item;
        }

        /// <summary>
        /// Create an arbitrary length sequence.
        /// </summary>
        /// <typeparam name="T">The generic type of the sequence.</typeparam>
        /// <param name="items">The elements that will end up in the sequence.</param>
        /// <returns>A sequence containing the elements passed as arguments.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="items"/> is <see langword="null"/></exception>
        public static IEnumerable<T> Create<T>(params T[] items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            return new System.Collections.ObjectModel.ReadOnlyCollection<T>(items);
        }

        /// <summary>
        /// Create a sequence that encapsulates another, avoiding the exposure of a direct instance.
        /// </summary>
        /// <typeparam name="T">The generic type of the sequence.</typeparam>
        /// <param name="enumerable">The original sequence to encapsulate.</param>
        /// <returns>A new sequence instance encapsulating the original.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IEnumerable<T> Seal<T>(IEnumerable<T> enumerable)
        {
            if (enumerable == null)
                throw new ArgumentNullException(nameof(enumerable));

            foreach (var item in enumerable)
                yield return item;
        }

        /// <inheritdoc cref="CombineRange{T}(IEnumerable{IEnumerable{T}})"/>
        /// <param name="enumerables">An array of the sequences to combine.</param>
        public static IEnumerable<IEnumerable<T>> Combine<T>(params IEnumerable<T>[] enumerables)
        {
            return CombineRange((IEnumerable<IEnumerable<T>>) enumerables);
        }

        /// <summary>
        /// Combine multiple sequences in every possible order of concatenation.
        /// </summary>
        /// <typeparam name="T">The generic type of the sequence.</typeparam>
        /// <param name="enumerables">A sequence of the sequences to combine.</param>
        /// <returns>
        /// A sequence containing the sequences that are the result of the concatenation of the provided sequences,
        /// one for each order of concatenation.
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IEnumerable<IEnumerable<T>> CombineRange<T>(IEnumerable<IEnumerable<T>> enumerables)
        {
            if (enumerables == null)
                throw new ArgumentNullException(nameof(enumerables));

            var current = Enumerable.Empty<IEnumerable<T>>().Append(Enumerable.Empty<T>());

            foreach (var enumerable in enumerables)
            {
                current = from c in current from e in enumerable select c.Append(e);
            }

            return current;
        }
    }
}
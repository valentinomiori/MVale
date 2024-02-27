using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace MVale.Core.Utils
{
    /// <summary>
    /// A utility class to perform conversions from bytes to hex strings, the other way around, and more.
    /// Related: <seealso cref="byte"/>, <seealso cref="char"/>, <seealso cref="string"/>
    /// </summary>
    public static class ByteHexUtil
    {
        /// <summary>
        /// A literal string as a reference for a lowercase positional hexadecimal base representation.
        /// </summary>
        public const string LowercaseHexAlphabet = "0123456789abcdef";

        /// <summary>
        /// A literal string as a reference for a uppercase positional hexadecimal base representation.
        /// </summary>
        public const string UppercaseHexAlphabet = "0123456789ABCDEF";

        /// <summary>
        /// An immutable set of lowercase characters constructed from <see cref="LowercaseHexAlphabet"/>.
        /// </summary>
        public static readonly ImmutableHashSet<char> LowercaseHexCharSet;

        /// <summary>
        /// An immutable set of uppercase characters constructed from <see cref="UppercaseHexAlphabet"/>.
        /// </summary>
        public static readonly ImmutableHashSet<char> UppercaseHexCharSet;

        /// <summary>
        /// An immutable set of lowercase and uppercase characters
        /// constructed merging <see cref="LowercaseHexCharSet"/> and <see cref="UppercaseHexCharSet"/>.
        /// </summary>
        public static readonly ImmutableHashSet<char> HexCharSet;

        /// <summary>
        /// An immutable array that can map the index (the byte integer value)
        /// to it's corresponding lowercase hexadecimal string representation.
        /// </summary>
        public static readonly ImmutableArray<string> UppercaseTranslationTable;

        /// <summary>
        /// An immutable array that can map the index (the byte integer value)
        /// to it's corresponding uppercase hexadecimal string representation.
        /// </summary>
        public static readonly ImmutableArray<string> LowercaseTranslationTable;

        /// <summary>
        /// An immutable map from a pair of hexadecimal format characters (MSB, LSB) to the corresponding byte value.
        /// </summary>
        public static readonly ImmutableDictionary<(char, char), byte> TranslationTable;

        static ByteHexUtil()
        {
            LowercaseHexCharSet = LowercaseHexAlphabet.ToImmutableHashSet();
            UppercaseHexCharSet = UppercaseHexAlphabet.ToImmutableHashSet();
            HexCharSet = Enumerable.Concat(LowercaseHexAlphabet, UppercaseHexAlphabet).ToImmutableHashSet();

            LowercaseTranslationTable = BuildTable(ToLowercaseHexChars);
            UppercaseTranslationTable = BuildTable(ToUppercaseHexChars);
            TranslationTable = BuildTranslationTable();
        }

        private static ImmutableArray<string> BuildTable(Func<byte, (char, char)> toHex)
        {
            const int TableLength = byte.MaxValue + 1;

            var builder = ImmutableArray.CreateBuilder<string>(TableLength);

            for (int i = 0; i < TableLength; i++)
            {
                var chars = toHex(checked((byte) i));

                builder.Add(new string(new char[] { chars.Item1, chars.Item2 }));
            }

            return builder.MoveToImmutable();
        }

        private static ImmutableDictionary<(char, char), byte> BuildTranslationTable()
        {
            const int TableLength = byte.MaxValue + 1;

            var builder = ImmutableDictionary.CreateBuilder<(char, char), byte>();

            for (int i = 0; i < TableLength; i++)
            {
                byte b = checked((byte) i);

                var charsList = EnumerableUtil.Create(
                    ToHexChars(b, false),
                    ToHexChars(b, true));

                var chars
                    = (from chars1 in charsList from chars2 in charsList
                    from combination in EnumerableUtil.Create(
                        (chars1.Item1, chars1.Item2),
                        (chars1.Item1, chars2.Item2),
                        (chars2.Item1, chars1.Item2),
                        (chars2.Item1, chars2.Item2))
                    select combination)
                    .ToImmutableHashSet();

                foreach (var combination in chars)
                {
                    builder.Add(combination, b);
                }
            }

            return builder.ToImmutable();
        }

        // Slightly less performant but uses (theoretically) super reliable code.
        internal static string SafeToHex(byte[] data)
            => BitConverter.ToString(data).Replace("-", string.Empty);

        /// <summary>
        /// Check if a character is a valid lowercase representation of a hexadecimal byte part.
        /// </summary>
        /// <param name="c">The character value.</param>
        /// <returns><see langword="true"/> if it is a valid, <see langword="false"/> otherwise.</returns>
        public static bool IsValidLowercaseHexChar(char c)
            => LowercaseHexCharSet.Contains(c);
        /// <summary>
        /// Check if a character is a valid uppercase representation of a hexadecimal byte part.
        /// </summary>
        /// <param name="c">The character value.</param>
        /// <returns><see langword="true"/> if it is a valid, <see langword="false"/> otherwise.</returns>
        public static bool IsValidUppercaseHexChar(char c)
            => UppercaseHexCharSet.Contains(c);
        /// <summary>
        /// Check if a character is a valid lowercase or uppercase representation of a hexadecimal byte part.
        /// </summary>
        /// <param name="c">The character value.</param>
        /// <returns><see langword="true"/> if it is a valid, <see langword="false"/> otherwise.</returns>
        public static bool IsValidHexChar(char c)
            => HexCharSet.Contains(c);

        /// <summary>
        /// Convert a byte to a pair of lowercase characters that are an hexadecimal representation.
        /// </summary>
        /// <param name="b">The byte value.</param>
        /// <returns>A tuple containing two characters representation (MSB, LSB).</returns>
        public static (char, char) ToLowercaseHexChars(byte b)
        {
            return (
                LowercaseHexAlphabet[(int)((b & 0xF0) >> 4)],
                LowercaseHexAlphabet[(int)(b & 0xF)]);
        }

        /// <summary>
        /// Convert a byte to a pair of uppercase characters that are an hexadecimal representation.
        /// </summary>
        /// <param name="b">The byte value.</param>
        /// <returns>A tuple containing two characters representation (MSB, LSB).</returns>
        public static (char, char) ToUppercaseHexChars(byte b)
        {
            return (
                UppercaseHexAlphabet[(int)((b & 0xF0) >> 4)],
                UppercaseHexAlphabet[(int)(b & 0xF)]);
        }

        /// <summary>
        /// Convert a byte to a pair of lowercase or uppercase characters that are an hexadecimal representation,
        /// depending on <paramref name="uppercase"/>.
        /// <para/> Uses <see cref="ToLowercaseHexChars(byte)"/> or <see cref="ToUppercaseHexChars(byte)"/>.
        /// </summary>
        /// <param name="b">The byte value.</param>
        /// <param name="uppercase"><see langword="true"/> for uppercase, <see langword="false"/> for lowercase.</param>
        /// <returns>A tuple containing two characters representation (MSB, LSB).</returns>
        public static (char, char) ToHexChars(byte b, bool uppercase = false)
        {
            if (uppercase)
                return ToUppercaseHexChars(b);
            else
                return ToLowercaseHexChars(b);
        }

        /// <summary>
        /// Convert a byte to a string that is the hexadecimal representation.
        /// <para/> Uses <see cref="ToHex(IEnumerable{byte}, bool)"/> with the <see langword="default"/> uppercase parameter.
        /// </summary>
        /// <param name="b">The byte value.</param>
        /// <returns>A string in hex format.</returns>
        public static string ToHex(byte b)
            => ToHex(EnumerableUtil.Create(b));

        /// <summary>
        /// Convert the byte parameters to a string that is the concatenated hexadecimal representation.
        /// <para/> Uses <see cref="ToHex(IEnumerable{byte}, bool)"/> with the <see langword="default"/> uppercase parameter.
        /// </summary>
        /// <param name="bytes">The byte values.</param>
        /// <returns>A string in hex format.</returns>
        public static string ToHex(params byte[] bytes)
            => ToHex((IEnumerable<byte>) bytes);

        /// <summary>
        /// Convert the byte parameters to a lowercase string that is the concatenated hexadecimal representation.
        /// <para/> Uses <see cref="ToHex(IEnumerable{byte}, bool)"/> with a <see langword="false"/> uppercase parameter.
        /// </summary>
        /// <param name="bytes">The byte values.</param>
        /// <returns>A string in hex format.</returns>
        public static string ToLowercaseHex(params byte[] bytes)
            => ToHex((IEnumerable<byte>) bytes, uppercase: false);

        /// <summary>
        /// Convert the byte parameters to an uppercase string that is the concatenated hexadecimal representation.
        /// <para/> Uses <see cref="ToHex(IEnumerable{byte}, bool)"/> with a <see langword="true"/> uppercase parameter.
        /// </summary>
        /// <param name="bytes">The byte values.</param>
        /// <returns>A string in hex format.</returns>
        public static string ToUppercaseHex(params byte[] bytes)
            => ToHex((IEnumerable<byte>) bytes, uppercase: true);

        /// <summary>
        /// Convert a sequence of bytes to a lowercase or uppercase string that is the concatenated hexadecimal representation.
        /// </summary>
        /// <param name="bytes">A sequence of bytes.</param>
        /// <param name="uppercase"><see langword="true"/> for uppercase, <see langword="false"/> for lowercase.</param>
        /// <returns>A string in hex format.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="bytes"/> is <see langword="null"/>.</exception>
        public static string ToHex(IEnumerable<byte> bytes, bool uppercase = false)
        {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));

            StringBuilder sb;
            if (bytes is IReadOnlyCollection<byte> collection)
            {
                sb = new StringBuilder(collection.Count * 2);
            }
            else
            {
                sb = new StringBuilder();
            }

            foreach (byte b in bytes)
            {
                var table = uppercase ? UppercaseTranslationTable : LowercaseTranslationTable;
                sb.Append(table[(int) b]);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Convert a hexadecimal representation string to a sequence of bytes.
        /// </summary>
        /// <param name="s">A string in hex format.</param>
        /// <returns>The corresponding sequence of bytes.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="s"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="s"/> contains invalid characters.</exception>
        public static IEnumerable<byte> FromHex(string s)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));

            for (int i = 0, j = 0; j < s.Length; i += 1, j += 2)
            {
                var key = (s[j], (j + 1 < s.Length) ? s[j + 1] : '0');

                byte b = default;
                try
                {
                    b = TranslationTable[key];
                }
                catch (KeyNotFoundException)
                {
                    if (!IsValidHexChar(key.Item1))
                        throw new ArgumentException(
                            $"String contains invalid char ({key.Item1:X2}) at position: {i}.",
                            nameof(s));

                    if (!IsValidHexChar(key.Item2))
                        throw new ArgumentException(
                            $"String contains invalid char ({key.Item2:X2}) at position: {i + 1}.",
                            nameof(s));
                }

                yield return b;
            }
        }

        /// <summary>
        /// Convert a hexadecimal representation string to an array of bytes.
        /// <para/> Uses <see cref="FromHex(string)"/> but is an optimization for a non deferred collection.
        /// </summary>
        /// <param name="s">A string in hex format.</param>
        /// <returns>The corresponding array of bytes.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="s"/> is <see langword="null"/>.</exception>
        public static byte[] ArrayFromHex(string s)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));

            byte[] bytes = new byte[s.Length / 2];

            int i = 0;
            foreach (byte b in FromHex(s))
            {
                bytes[i] = b;
                i++;
            }

            return bytes;
        }

        /// <summary>
        /// Convert a hexadecimal representation string to an array of bytes.
        /// <para/> Uses <see cref="FromHex(string)"/> but is an optimization for a non deferred immutable collection.
        /// </summary>
        /// <param name="s">A string in hex format.</param>
        /// <returns>The corresponding immutable array of bytes.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="s"/> is <see langword="null"/>.</exception>
        public static ImmutableArray<byte> ImmutableArrayFromHex(string s)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));

            var builder = ImmutableArray.CreateBuilder<byte>(s.Length / 2);

            foreach (byte b in FromHex(s))
            {
                builder.Add(b);
            }

            return builder.MoveToImmutable();
        }
    }
}

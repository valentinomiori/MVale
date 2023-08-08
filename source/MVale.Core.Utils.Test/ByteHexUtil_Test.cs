using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace MVale.Core.Utils.Test
{
    [TestOf(typeof(ByteHexUtil))]
    public class ByteHexUtil_Test
    {
        private static IEnumerable<TestCaseData> ByteToLowercaseHexChars
        {
            get
            {
                return EnumerableUtil.ByteValues.Select(b =>
                {
                    string s = ToHex(b).ToLowerInvariant();
                    return new TestCaseData(b, (s[0], s[1]));
                });
            }
        }

        private static IEnumerable<TestCaseData> ByteToUppercaseHexChars
        {
            get
            {
                return EnumerableUtil.ByteValues.Select(b =>
                {
                    string s = ToHex(b).ToUpperInvariant();
                    return new TestCaseData(b, (s[0], s[1]));
                });
            }
        }

        private static string ToHex(byte b)
        {
            return b.ToString("X2");
        }

        private static string ToHex(byte[] bytes)
        {
            return string.Concat(bytes.Select(ToHex));
        }

        private static IEnumerable<byte[]> GenerateBytes(int length = 1, int count = 0)
        {
            static IEnumerable<byte> GenerateRandomBytes(int count = 0)
            {
                for (int i = 0; i < count; i++)
                    yield return TestContext.CurrentContext.Random.NextByte();
            }

            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                    yield return GenerateRandomBytes(length).ToArray();
            }
            else
            {
                for (int i = 0; i < length; i++)
                {
                    foreach (var e in EnumerableUtil.CombineRange(Enumerable.Repeat(EnumerableUtil.ByteValues, length)))
                        yield return e.ToArray();
                }
            }
        }

        private static IEnumerable<TestCaseData> CreateBytesToHexStringPairs(
            int length = 1,
            int count = 0,
            bool? uppercase = null)
        {
            foreach (var bytes in GenerateBytes(length, count))
            {
                var set = new HashSet<string>();

                if (uppercase ?? false == false)
                {
                    set.Add(ToHex(bytes).ToLowerInvariant());
                }

                if (uppercase ?? true == true)
                {
                    set.Add(ToHex(bytes).ToUpperInvariant());
                }

                foreach (var s in set)
                    yield return new TestCaseData(bytes, s);
            }
        }

        [Test]
        public void LowercaseHexCharset_Test()
        {
            Assert.IsTrue(
                ByteHexUtil.LowercaseHexCharSet.All(c => ByteHexUtil.LowercaseHexAlphabet.Contains(c)));
        }

        [Test]
        public void UppercaseHexCharset_Test()
        {
            Assert.IsTrue(
                ByteHexUtil.UppercaseHexCharSet.All(c => ByteHexUtil.UppercaseHexAlphabet.Contains(c)));
        }

        [Test]
        public void TranslationTable_Test()
        {
            Assert.IsTrue(ByteHexUtil.TranslationTable.Keys.All(
                chars => ByteHexUtil.HexCharSet.Contains(chars.Item1)
                    && ByteHexUtil.HexCharSet.Contains(chars.Item2)));
        }

        [TestCaseSource(nameof(ByteToLowercaseHexChars))]
        public void ToLowercaseHexChars_Test(byte b, (char, char) chars)
        {
            Assert.AreEqual(ByteHexUtil.ToLowercaseHexChars(b), chars);
        }

        [TestCaseSource(nameof(ByteToUppercaseHexChars))]
        public void ToUppercaseHexChars_Test(byte b, (char, char) chars)
        {
            Assert.AreEqual(ByteHexUtil.ToUppercaseHexChars(b), chars);
        }

        [Test]
        public void ToHexChars_Test()
        {
            Assert.AreEqual(ByteHexUtil.ToHexChars(0x00), ('0', '0'));

            Assert.AreEqual(ByteHexUtil.ToLowercaseHexChars(0x0A), ('0', 'a'));
            Assert.AreEqual(ByteHexUtil.ToLowercaseHexChars(0x0A), ByteHexUtil.ToHexChars(0x0A, false));
            Assert.AreEqual(ByteHexUtil.ToUppercaseHexChars(0x0F), ('0', 'F'));
            Assert.AreEqual(ByteHexUtil.ToUppercaseHexChars(0x0F), ByteHexUtil.ToHexChars(0x0F, true));
        }

        [TestCaseSource(nameof(ByteToLowercaseHexChars))]
        public void ToHexChars_Test_Lowercase(byte b, (char, char) chars)
        {
            Assert.AreEqual(ByteHexUtil.ToHexChars(b, uppercase: false), chars);
        }

        [TestCaseSource(nameof(ByteToUppercaseHexChars))]
        public void ToHexChars_Test_Uppercase(byte b, (char, char) chars)
        {
            Assert.AreEqual(ByteHexUtil.ToHexChars(b, uppercase: true), chars);
        }

        [Test]
        public void ToHex_Test()
        {
            Assert.AreEqual(ByteHexUtil.ToHex(0x11), "11");
        }

        [TestCaseSource(nameof(CreateBytesToHexStringPairs), new object[] { 1, 0, null })]
        public void ToHex_Test(byte[] bytes, string s)
        {
            Console.WriteLine($"Verifying value: {s}.");

            Assert.AreEqual(ByteHexUtil.ToHex(bytes, uppercase: false), s.ToLowerInvariant());
            Assert.AreEqual(ByteHexUtil.ToHex(bytes, uppercase:  true), s.ToUpperInvariant());
        }

        [Test]
        public void FromHex_Test()
        {
            Assert.That(
                () => ByteHexUtil.FromHex("11")
                    .SequenceEqual(EnumerableUtil.Create((byte) 0x11)));

            ArgumentException exception = null;
            try
            {
                // The result should be iterated to produce certain exceptions.
                ByteHexUtil.FromHex("1Y").ToList();
            }
            catch (ArgumentException e)
            {
                exception = e;
            }

            if (exception != null)
            {
                Assert.AreEqual(exception.ParamName, "s");
            }
            else
            {
                Assert.Fail("The code should have thrown an exception.");
            }
        }
    }
}
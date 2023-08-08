using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace MVale.Core.Utils.Test
{
    [TestOf(typeof(EnumerableUtil))]
    public class EnumerableUtil_Test
    {
        [Test]
        public void ByteValues_Test()
        {
            Assert.AreEqual(EnumerableUtil.ByteValues.ToHashSet().Count, 256);
        }

        [Test]
        public void SByteValues_Test()
        {
            Assert.AreEqual(EnumerableUtil.SByteValues.ToHashSet().Count, 256);
        }

        [Test]
        public void Combine_Test()
        {
            var chars = new HashSet<char>()
            {
                'A',
                'B'
            };

            var set = EnumerableUtil.Combine(chars, chars)
            .Select(e => string.Concat(e))
            .ToHashSet();

            var expected = new HashSet<string>()
            { 
                "AA",
                "AB",
                "BA",
                "BB"
            };

            Assert.AreEqual(set, expected);
        }
    }
}
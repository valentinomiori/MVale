using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace MVale.Core.Test
{
    [TestFixture(TestOf = typeof(InternalHashCodeUtil))]
    public class HashCodeUtility_Test
    {
        [Test]
        public void TestCustomCombineRange()
        {
            static int Expected<T>(IEnumerable<T> values)
            {
                int hc = 17;

                foreach (var value in values)
                {
                    hc = unchecked (hc * 31 + (value?.GetHashCode() ?? 0));
                }

                return hc;
            }

            var values = new[] { 44L, 77L };
            int hc = InternalHashCodeUtil.CustomCombineRange(values);
            Assert.AreEqual(Expected(values), hc);
        }
    }
}
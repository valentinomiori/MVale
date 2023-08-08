using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace MVale.Core.Utils.Test
{
    [TestOf(typeof(EnumUtil))]
    public class EnumUtil_Test
    {
        public static IEnumerable<TestCaseData> IsNullableEnum_TestCaseData
        {
            get
            {
                static TestCaseData Create(Type type, bool result)
                    => new TestCaseData(type, result);

                yield return Create(typeof(string), false);
                yield return Create(typeof(int), false);
                yield return Create(typeof(int?), false);
                yield return Create(typeof(StringComparison), false);
                yield return Create(typeof(StringComparison?), true);
            }
        }

        public static IEnumerable<TestCaseData> GetUnderlyingValue_TestCaseData
        {
            get
            {
                static TestCaseData Create(object @object, object result)
                    => new TestCaseData(@object, result);

                yield return Create(null, null);
                
                foreach (var value in Enum.GetValues(typeof(StringComparison)))
                    yield return Create(value, (int) value);

                foreach (var value in Enum.GetValues(typeof(System.Reflection.BindingFlags)))
                    yield return Create(value, (int) value);
            }
        }

        [Test]
        public void IsNullableEnum_Test()
        {
            Assert.Throws<ArgumentNullException>(() => EnumUtil.IsNullableEnum(null));
        }

        [TestCaseSource(nameof(IsNullableEnum_TestCaseData))]
        public void IsNullableEnum_Test(Type type, bool result)
        {
            Assert.AreEqual(result, EnumUtil.IsNullableEnum(type));
        }

        [Test]
        public static void GetUnderlyingValue_Test()
        {
            Assert.Throws<ArgumentException>(() => EnumUtil.GetUnderlyingValue("A string."));
        }

        [TestCaseSource(nameof(GetUnderlyingValue_TestCaseData))]
        public static void GetUnderlyingValue_Test(object @object, object result)
        {
            Assert.AreEqual(result, EnumUtil.GetUnderlyingValue(@object));
            Assert.AreEqual(result, EnumUtil.GetUnderlyingValue((Enum) @object));
        }
    }
}
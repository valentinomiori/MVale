using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace MVale.Core.Utils.Test
{
    [TestOf(typeof(TypeUtil))]
    public class TypeUtil_Test
    {
        public class TestClass
        {
            public const int ConstValue = 18273645;

            public static implicit operator int(TestClass test)
            {
                return ConstValue;
            }
        }

        public class DerivedTestClass : TestClass
        {
            public new const int ConstValue = 81726354;

            public static implicit operator int(DerivedTestClass test)
            {
                return ConstValue;
            }
        }

        public static IEnumerable<TestCaseData> CastTo_TestCaseSource
        {
            get
            {
                static TestCaseData Create(object instance, Type type, bool isChecked, object result)
                    => new TestCaseData(instance, type, isChecked, result);

                yield return Create(StringComparison.Ordinal, typeof(int), true, (int) StringComparison.Ordinal);
                yield return Create((int) StringComparison.Ordinal, typeof(StringComparison), true, StringComparison.Ordinal);
                yield return Create(1, typeof(int), true, 1);
                yield return Create("A string.", typeof(string), true, "A string.");
                yield return Create(null, typeof(char?), true, null);
            }
        }

        public static IEnumerable<TestCaseData> DynamicCastTo_TestCaseSource
        {
            get
            {
                static TestCaseData Create(object instance, Type currentType, Type desiredType, bool isChecked, object result)
                    => new TestCaseData(instance, currentType, desiredType, isChecked, result);

                foreach (var tcd in CastTo_TestCaseSource)
                    yield return new TestCaseData(
                        tcd.Arguments[0],
                        tcd.Arguments[0]?.GetType() ?? typeof(object),
                        tcd.Arguments[1],
                        tcd.Arguments[2],
                        tcd.Arguments[3]);

                yield return Create(StringComparison.Ordinal, typeof(StringComparison), typeof(long), true, (long) StringComparison.Ordinal);
                yield return Create(11, typeof(int), typeof(float), true, 11.0f);
                yield return Create(int.MaxValue + 1L, typeof(long), typeof(int), false, int.MinValue);
                yield return Create(new TestClass(), typeof(TestClass), typeof(int), true, TestClass.ConstValue);
                yield return Create(new DerivedTestClass(), typeof(TestClass), typeof(int), true, TestClass.ConstValue);
                yield return Create(new DerivedTestClass(), typeof(DerivedTestClass), typeof(int), true, DerivedTestClass.ConstValue);
            }
        }

        [Test]
        public static void CastTo_Test()
        {
            Assert.Throws<ArgumentNullException>(() => TypeUtil.CastTo(1, null, true));
            Assert.Throws<InvalidCastException>(() => TypeUtil.CastTo("A string.", typeof(byte), true));
            Assert.Throws<InvalidCastException>(() => TypeUtil.CastTo(1, typeof(float), true));
            Assert.Throws<InvalidCastException>(() => TypeUtil.CastTo(new TestClass(), typeof(int), true));
            Assert.Throws<InvalidCastException>(() => TypeUtil.CastTo(new TestClass(), typeof(DerivedTestClass)));
        }

        [TestCaseSource(nameof(CastTo_TestCaseSource))]
        public static void CastTo_Test(object instance, Type type, bool isChecked, object result)
        {
            var actual = TypeUtil.CastTo(instance, type, isChecked);
            Assert.AreEqual(result?.GetType(), actual?.GetType());
            Assert.AreEqual(result, actual);
        }

        [Test]
        public static void ConvertTo_Test()
        {
            Assert.Throws<ArgumentNullException>(() => TypeUtil.ConvertTo(1, typeof(int), null, true));
            Assert.Throws<ArgumentNullException>(() => TypeUtil.ConvertTo(1, null, typeof(int), true));
            Assert.Throws<ArgumentNullException>(() => TypeUtil.ConvertTo(1, null, null, true));
            Assert.Throws<ArgumentException>(() => TypeUtil.ConvertTo(null, typeof(int), typeof(long), null));
            Assert.Throws<ArgumentException>(() => TypeUtil.ConvertTo(new TestClass(), typeof(DerivedTestClass), typeof(object)));
            Assert.Throws<InvalidCastException>(() => TypeUtil.ConvertTo(new TestClass(), typeof(TestClass), typeof(DerivedTestClass)));
            Assert.Throws<InvalidCastException>(() => TypeUtil.ConvertTo("A string.", typeof(object), typeof(byte), true));
            Assert.Throws<OverflowException>(() => TypeUtil.ConvertTo(int.MaxValue + 1L, typeof(long), typeof(int), true));
        }

        [TestCaseSource(nameof(DynamicCastTo_TestCaseSource))]
        public static void ConvertTo_Test(object instance, Type currentType, Type desiredType, bool isChecked, object result)
        {
            object actual = TypeUtil.ConvertTo(instance, currentType, desiredType, isChecked);
            Assert.AreEqual(result?.GetType(), actual?.GetType());
            Assert.AreEqual(result, actual);
        }
    }
}
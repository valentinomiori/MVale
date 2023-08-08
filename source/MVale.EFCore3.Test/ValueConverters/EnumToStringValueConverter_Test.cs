using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace MVale.EntityFramework.Test.ValueConverters
{
    using Test_Subject = MVale.EntityFramework.ValueConverters.EnumToStringValueConverter<EnumToStringValueConverter_Test.TestEnum>;

    [TestOf(typeof(Test_Subject))]
    public class EnumToStringValueConverter_Test
    {
        public enum TestEnum : byte
        {
            Value_0 = 0,
            Value_0A = 0,
            Value_1 = 1
        }

        public static IEnumerable<TestCaseData> TestValues
        {
            get
            {
                foreach (var value in Enum.GetValues(typeof(TestEnum)).Cast<TestEnum>())
                    yield return new TestCaseData(value);

                yield return new TestCaseData((TestEnum) byte.MaxValue);
            }
        }

        private Test_Subject CreateSubject()
        {
            return new Test_Subject();
        }

        private void InternalTestReconvertedEqualsOriginal<TEnum>(TEnum value)
        where TEnum : struct, Enum
        {
            var subject = this.CreateSubject();

            var converted = subject.ConvertToProvider(value);
            Assert.That(() => converted is string);
            Assert.AreEqual(value.ToString(), (string) converted);

            var reconverted = subject.ConvertFromProvider((string) converted);
            Assert.That(() => reconverted is TEnum);
            Assert.AreEqual(value, (TEnum) reconverted);
        }

        [TestCaseSource(nameof(TestValues))]
        public void TestReconvertedEqualsOriginal(TestEnum value)
        {
            this.InternalTestReconvertedEqualsOriginal(value);
        }
    }
}
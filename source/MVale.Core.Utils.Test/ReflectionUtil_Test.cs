using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace MVale.Core.Utils.Test
{
    [TestOf(typeof(EnumerableUtil))]
    public class ReflectionUtil_Test
    {
        interface I1
        {
            public static string I1StaticField = nameof(I1StaticField);
        }

        class A : I1
        {
            public static FieldInfo NonPublicAFieldInfo => ReflectionUtil.Field((A a) => a.NonPublicAField);

            public static string AStaticField = nameof(AStaticField);
            private string NonPublicAField = nameof(NonPublicAField);
            public readonly string AField = nameof(AField);
        }
        
        class B : A
        {
            public static FieldInfo BFieldInfo => ReflectionUtil.Field((B b) => b.BField);

            protected string BField = nameof(BField);
        }
        
        class C : B
        {
            public string CField = nameof(CField);
        }

        [Test]
        public void GetBaseTypes_Test()
        {
            var expected = new HashSet<Type>()
            {
                typeof(B),
                typeof(A),
                typeof(object)
            };

            var actual = ReflectionUtil.GetBaseTypes(typeof(C)).ToHashSet();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetAllFields_Test1()
        {
            var expected = new HashSet<FieldInfo>()
            {
                ReflectionUtil.Field((C c) => c.CField),
                ReflectionUtil.Field(() => A.AStaticField),
                ReflectionUtil.Field((A a) => a.AField)
            };

            var actual = ReflectionUtil.GetAllFields(typeof(C)).ToHashSet();

            Assert.That(expected.SetEquals(actual), string.Join(", ", expected) + " | " + string.Join(", ", actual));
        }

        [Test]
        public void GetAllFields_Test2()
        {
            var expected = new HashSet<FieldInfo>()
            {
                ReflectionUtil.Field((C c) => c.CField),
                B.BFieldInfo,
                ReflectionUtil.Field(() => A.AStaticField),
                A.NonPublicAFieldInfo,
                ReflectionUtil.Field((A a) => a.AField)
            }
            .ToHashSet();

            const BindingFlags bindingAttr
                = BindingFlags.Public
                | BindingFlags.NonPublic
                | BindingFlags.Instance
                | BindingFlags.Static;

            var actual = ReflectionUtil.GetAllFields(typeof(C), bindingAttr).ToHashSet();

            Assert.That(expected.SetEquals(actual), string.Join(", ", expected) + " | " + string.Join(", ", actual));
        }
    }
}
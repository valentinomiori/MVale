using System;
using NUnit.Framework;

namespace MVale.Core.Test.Collections
{
    [TestFixture(TestOf = typeof(Core.Collections.OrderedDictionary<,>))]
    public class OrderedDictionary_Test
    {
        public static System.Collections.Generic.Dictionary<float, string> TestDictionary
            => new System.Collections.Generic.Dictionary<float, string>()
            {
                [100f] = "A",
                [200f] = "B",
                [300f] = "C"
            };

        public static System.Collections.Generic.Dictionary<float, string> DeviatedTestDictionary
            => new System.Collections.Generic.Dictionary<float, string>()
            {
                [100.4f] = "A",
                [200f] = "B",
                [300f] = "C"
            };

        public static void AddRange<TKey, TValue>(
            System.Collections.Generic.IDictionary<TKey, TValue> d,
            System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<TKey, TValue>> pairs)
        {
            foreach (var pair in pairs)
            {
                d.Add(pair);
            }
        }

        public void AssertEqual<TKey, TValue>(
            Core.Collections.OrderedDictionary<TKey, TValue> od, 
            System.Collections.Generic.IDictionary<TKey, TValue> d)
        {
            Assert.AreEqual(od.Count, d.Count);

            d = new System.Collections.Generic.Dictionary<TKey, TValue>(d, od.Comparer);

            foreach (var pair in od)
            {
                Assert.That(() => d.ContainsKey(pair.Key));
                Assert.That(() => Core.Collections.Comparers.EqualityComparer<TValue>.Default.Equals(pair.Value, d[pair.Key]));
            }
        }

        [Test]
        public void Ctors_Test()
        {
            var od = new Core.Collections.OrderedDictionary<float, string>();

            od = new Core.Collections.OrderedDictionary<float, string>(TestDictionary);
            this.AssertEqual(od, TestDictionary);

            od = new Core.Collections.OrderedDictionary<float, string>(Core.Collections.Comparers.RoundedFloatEqualityComparer.HalfToEven);
            AddRange(od, DeviatedTestDictionary);
            this.AssertEqual(od, TestDictionary);

            od = new Core.Collections.OrderedDictionary<float, string>(100);

            od = new Core.Collections.OrderedDictionary<float, string>(
                DeviatedTestDictionary,
                Core.Collections.Comparers.RoundedFloatEqualityComparer.HalfToEven);
            this.AssertEqual(od, TestDictionary);

            od = new Core.Collections.OrderedDictionary<float, string>(
                0,
                Core.Collections.Comparers.RoundedFloatEqualityComparer.HalfToEven);
            AddRange(od, DeviatedTestDictionary);
            this.AssertEqual(od, TestDictionary);
        }

        [Test]
        public void Serializable_Test()
        {
            var od = new Core.Collections.OrderedDictionary<float, string>(
                TestDictionary,
                Core.Collections.Comparers.RoundedFloatEqualityComparer.HalfToEven);
#pragma warning disable SYSLIB0011
            System.Runtime.Serialization.IFormatter formatter
                = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            System.IO.Stream s = new System.IO.MemoryStream();
#pragma warning restore
            formatter.Serialize(s, od);

            s.Seek(0, System.IO.SeekOrigin.Begin);
            var odDeserialized = (Core.Collections.OrderedDictionary<float, string>) formatter.Deserialize(s);

            this.AssertEqual(od, odDeserialized);
        }
    }
}
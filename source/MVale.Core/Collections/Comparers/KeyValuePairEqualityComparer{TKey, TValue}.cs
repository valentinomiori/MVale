using System;
using System.Collections.Generic;

namespace MVale.Core.Collections.Comparers
{
    [Serializable]
    public sealed class KeyValuePairEqualityComparer<TKey, TValue> :
        EqualityComparer<KeyValuePair<TKey, TValue>>
    {
        public System.Collections.Generic.IEqualityComparer<TKey> KeyComparer { get; }

        public System.Collections.Generic.IEqualityComparer<TValue> ValueComparer { get; }


        public KeyValuePairEqualityComparer(System.Collections.Generic.IEqualityComparer<TKey> keyComparer) :
            this(keyComparer, null)
        {
        }

        public KeyValuePairEqualityComparer(System.Collections.Generic.IEqualityComparer<TValue> valueComparer) :
            this(null, valueComparer)
        {
        }

        public KeyValuePairEqualityComparer(
            System.Collections.Generic.IEqualityComparer<TKey> keyComparer,
            System.Collections.Generic.IEqualityComparer<TValue> valueComparer)
        {
            KeyComparer = keyComparer ?? System.Collections.Generic.EqualityComparer<TKey>.Default;
            ValueComparer = valueComparer ?? System.Collections.Generic.EqualityComparer<TValue>.Default;
        }

        public override bool Equals(KeyValuePair<TKey, TValue> x, KeyValuePair<TKey, TValue> y)
        {
            return this.KeyComparer.Equals(x.Key, y.Key) && this.ValueComparer.Equals(x.Value, y.Value);
        }

        public override int GetHashCode(KeyValuePair<TKey, TValue> obj)
        {
            return (this.KeyComparer.GetHashCode(obj.Key), this.ValueComparer.GetHashCode(obj.Value)).GetHashCode();
        }
    }
}
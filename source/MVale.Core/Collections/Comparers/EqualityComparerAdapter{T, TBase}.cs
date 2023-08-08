using System;

namespace MVale.Core.Collections.Comparers
{
    [Serializable]
    public class EqualityComparerAdapter<T, TBase> : EqualityComparer<T>
        where T : TBase
    {
        public System.Collections.Generic.IEqualityComparer<TBase> Comparer { get; }

        public EqualityComparerAdapter(System.Collections.Generic.IEqualityComparer<TBase> comparer)
        {
            Comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
        }

        public override bool Equals(T x, T y)
        {
            return this.Comparer.Equals(x, y);
        }

        public override int GetHashCode(T obj)
        {
            return this.Comparer.GetHashCode(obj);
        }
    }
}
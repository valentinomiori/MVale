using System;

namespace MVale.Core.Collections.Comparers
{
    [Serializable]
    public class EqualityComparerAdapter<T> : EqualityComparer<T>
    {
        public System.Collections.Generic.IEqualityComparer<T> Comparer { get; }

        public EqualityComparerAdapter(System.Collections.Generic.IEqualityComparer<T> comparer)
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
using System;

namespace MVale.Core.Collections.Comparers
{
    [Serializable]
    public class EqualityComparerAdapter : IEqualityComparer<object>
    {
        public System.Collections.IEqualityComparer Comparer { get; }

        public EqualityComparerAdapter(System.Collections.IEqualityComparer comparer)
        {
            Comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
        }

        public new bool Equals(object x, object y)
        {
            return this.Comparer.Equals(x, y);
        }

        public int GetHashCode(object obj)
        {
            return this.Comparer.GetHashCode(obj);
        }
    }
}
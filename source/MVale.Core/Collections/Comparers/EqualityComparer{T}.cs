using System;
using System.Collections;

namespace MVale.Core.Collections.Comparers
{
    [Serializable]
    public abstract class EqualityComparer<T> : IEqualityComparer<T>
    {
        private static EqualityComparer<T> _default;

        public static EqualityComparer<T> Default
            => _default ??= new EqualityComparerAdapter<T>(System.Collections.Generic.EqualityComparer<T>.Default);

        public abstract bool Equals(T x, T y);

        public abstract int GetHashCode(T obj);

        bool IEqualityComparer.Equals(object x, object y)
        {
            return this.Equals(
                ThrowHelper.CastArgumentOrThrow<T>(x, nameof(x)),
                ThrowHelper.CastArgumentOrThrow<T>(y, nameof(y)));
        }

        int IEqualityComparer.GetHashCode(object obj)
        {
            return this.GetHashCode(ThrowHelper.CastArgumentOrThrow<T>(obj, nameof(obj)));
        }
    }
}
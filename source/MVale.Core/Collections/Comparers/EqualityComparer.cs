using System;
using System.Collections;

namespace MVale.Core.Collections.Comparers
{
    public static class EqualityComparer
    {
        public static IEqualityComparer<object> From(System.Collections.IEqualityComparer comparer)
        {
            if (comparer is IEqualityComparer<object> c)
                return c;

            return new EqualityComparerAdapter(comparer);
        }

        public static IEqualityComparer<T> From<T>(System.Collections.IEqualityComparer comparer)
            where T : struct
        {
            if (comparer is IEqualityComparer<T> c)
                return c;

            return new EqualityComparerAdapter<T, object>(From(comparer));
        }

        public static IEqualityComparer<T> From<T>(System.Collections.Generic.IEqualityComparer<T> comparer)
        {
            if (comparer is IEqualityComparer<T> c)
                return c;

            if (Object.ReferenceEquals(comparer, System.Collections.Generic.EqualityComparer<T>.Default))
                return EqualityComparer<T>.Default;

            return new EqualityComparerAdapter<T>(comparer);
        }
    }
}
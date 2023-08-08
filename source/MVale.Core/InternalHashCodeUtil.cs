using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MVale.Core
{
    internal static class InternalHashCodeUtil
    {
        public static int Combine<T>(params T[] values)
        {
            return CombineRange<T>(values);
        }

        public static int CombineRange(IEnumerable values)
        {
            return CombineRange<object>((values as IEnumerable<object>) ?? values?.Cast<object>());
        }

#if NETCOREAPP
        public static int CombineRange<T>(IEnumerable<T> values)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            var hc = new HashCode();

            foreach (var value in values)
            {
                hc.Add<T>(value);
            }

            return hc.ToHashCode();
        }
#endif

#if !NETCOREAPP
        public static int CombineRange<T>(IEnumerable<T> values)
#else
        internal static int CustomCombineRange<T>(IEnumerable<T> values)
#endif
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            const int seed = 17;
            const int fact = 31;

            int hc = seed;

            foreach (var value in values)
            {
                hc = unchecked(hc * fact + (value?.GetHashCode() ?? 0));
            }

            return hc;
        }
    }
}
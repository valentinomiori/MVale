using System;
using System.Collections;
using System.Collections.Generic;

namespace MVale.Core.Utils
{
    /// <summary>
    /// A utility class to ease the generation of hash codes..
    /// Related: <seealso cref="object.GetHashCode"/>, <seealso cref="IEqualityComparer{T}.GetHashCode(T)"/>
    /// </summary>
    public static class HashCodeUtil
    {
        public static int Combine<T>(params T[] values)
        {
            return Core.InternalHashCodeUtil.Combine(values);
        }

        public static int CombineRange(IEnumerable values)
        {
            return Core.InternalHashCodeUtil.CombineRange(values);
        }

        public static int CombineRange<T>(IEnumerable<T> values)
        {
            return Core.InternalHashCodeUtil.CombineRange(values);
        }
    }
}
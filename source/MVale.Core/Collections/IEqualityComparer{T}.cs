using System;

namespace MVale.Core.Collections
{
    public interface IEqualityComparer<in T> :
        System.Collections.Generic.IEqualityComparer<T>,
        System.Collections.IEqualityComparer
    {
    }
}
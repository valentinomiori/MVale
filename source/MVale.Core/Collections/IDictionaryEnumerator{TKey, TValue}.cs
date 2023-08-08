using System;
using System.Collections;
using System.Collections.Generic;

namespace MVale.Core.Collections
{
    public interface IDictionaryEnumerator<TKey, TValue> : IEnumerator<KeyValuePair<TKey, TValue>>, IDictionaryEnumerator
    {
        new TKey Key { get; }
        new TValue Value { get; }
    }
}
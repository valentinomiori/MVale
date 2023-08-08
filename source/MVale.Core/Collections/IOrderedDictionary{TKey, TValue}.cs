using System;
using System.Collections;
using System.Collections.Generic;

namespace MVale.Core.Collections
{
    public interface IOrderedDictionary<TKey, TValue> :
        IDictionary<TKey, TValue>,
        ICollection<KeyValuePair<TKey, TValue>>,
        IEnumerable<KeyValuePair<TKey, TValue>>,
        IEnumerable
    {
        public KeyValuePair<TKey, TValue> this[int index] { get; set; }

        void InsertAt(int index, TKey key, TValue value);
    }
}
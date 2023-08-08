using System;
using System.Collections;
using System.Collections.Generic;

namespace MVale.Core.Collections
{
    [Serializable]
    public abstract class DictionaryEnumerator<TKey, TValue> :
        IDictionaryEnumerator<TKey, TValue>,
        IEnumerator<KeyValuePair<TKey, TValue>>,
        IEnumerator,
        IDisposable,
        IDictionaryEnumerator
    {
        object IEnumerator.Current => this.Current;

        DictionaryEntry IDictionaryEnumerator.Entry
            => new DictionaryEntry(this.Key, this.Value);

        object IDictionaryEnumerator.Key => this.Key;

        object IDictionaryEnumerator.Value => this.Value;

        public abstract KeyValuePair<TKey, TValue> Current { get; }

        public TKey Key => this.Current.Key;

        public TValue Value => this.Current.Value;

        public abstract void Dispose();

        public abstract bool MoveNext();

        public abstract void Reset();
    }
}
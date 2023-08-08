using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MVale.Core.Collections
{
    [Serializable]
    public class OrderedDictionary<TKey, TValue> :
        IOrderedDictionary<TKey, TValue>,
        IDictionary<TKey, TValue>,
        ICollection<KeyValuePair<TKey, TValue>>,
        IEnumerable<KeyValuePair<TKey, TValue>>,
        System.Collections.IEnumerable,
        IReadOnlyDictionary<TKey, TValue>,
        IReadOnlyCollection<KeyValuePair<TKey, TValue>>,
        System.Collections.Specialized.IOrderedDictionary,
        IDictionary,
        ICollection,
        IDeserializationCallback,
        ISerializable
    {
#region Types
        public sealed class KeyCollection : ICollection<TKey>, IReadOnlyCollection<TKey>, ICollection
        {
            public OrderedDictionary<TKey, TValue> Dictionary { get; }

            bool ICollection<TKey>.IsReadOnly => true;

            bool ICollection.IsSynchronized => IsSynchronizedValue;

            object ICollection.SyncRoot
                => ((ICollection) this.Dictionary).SyncRoot;

            public int Count
                => this.Dictionary.Count;

            public KeyCollection(OrderedDictionary<TKey, TValue> dictionary)
            {
                Dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
            }

            void ICollection<TKey>.Add(TKey item)
                => throw new NotSupportedException();

            void ICollection<TKey>.Clear()
                => throw new NotSupportedException();

            bool ICollection<TKey>.Remove(TKey item)
                => throw new NotSupportedException();

            IEnumerator IEnumerable.GetEnumerator()
                => this.GetEnumerator();

            public bool Contains(TKey item)
                => this.Dictionary.ContainsKey(item);

            public void CopyTo(TKey[] array, int arrayIndex)
            {
                Utils.ReadOnlyCollectionUtil.CopyTo(this, array, arrayIndex);
            }

            public IEnumerator<TKey> GetEnumerator()
            {
                foreach (var item in this.Dictionary)
                    yield return item.Key;
            }

            void ICollection.CopyTo(Array array, int index)
            {
                Utils.ReadOnlyCollectionUtil.CopyTo(this, array, index);
            }
        }

        public sealed class ValueCollection : ICollection<TValue>, IReadOnlyCollection<TValue>, ICollection
        {
            public OrderedDictionary<TKey, TValue> Dictionary { get; }

            bool ICollection<TValue>.IsReadOnly => this.Dictionary.IsReadOnly;

            bool ICollection.IsSynchronized => IsSynchronizedValue;

            object ICollection.SyncRoot
                => ((ICollection) this.Dictionary).SyncRoot;

            public int Count
                => this.Dictionary.Count;

            public ValueCollection(OrderedDictionary<TKey, TValue> dictionary)
            {
                Dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
            }

            void ICollection<TValue>.Add(TValue item)
                => throw new NotSupportedException();

            void ICollection<TValue>.Clear()
                => throw new NotSupportedException();

            bool ICollection<TValue>.Remove(TValue item)
                => throw new NotSupportedException();

            IEnumerator IEnumerable.GetEnumerator()
                => this.GetEnumerator();

            public bool Contains(TValue item)
                => this.Dictionary.ContainsValue(item);

            public void CopyTo(TValue[] array, int arrayIndex)
            {
                Utils.ReadOnlyCollectionUtil.CopyTo(this, array, arrayIndex);
            }

            public IEnumerator<TValue> GetEnumerator()
            {
                foreach (var item in this.Dictionary)
                    yield return item.Value;
            }

            void ICollection.CopyTo(Array array, int index)
            {
                Utils.ReadOnlyCollectionUtil.CopyTo(this, array, index);
            }
        }
#endregion

        private const bool IsSynchronizedValue = false;

#region Fields
        private readonly SerializationInfo serializationInfo;

        private readonly StreamingContext streamingContext;

        private object syncRoot;

        private List<KeyValuePair<TKey, TValue>> list;

        private Dictionary<TKey, int> dictionary;

        private IEqualityComparer<TKey> comparer;

        private KeyCollection keys;

        private ValueCollection values;
#endregion

#region Properties
        bool IDictionary.IsFixedSize => false;

        bool ICollection.IsSynchronized => IsSynchronizedValue;

        object ICollection.SyncRoot
        {
            get
            {
                if (this.syncRoot == null)
                {
                    System.Threading.Interlocked.CompareExchange<object>(ref this.syncRoot, new object(), null);    
                }

                return this.syncRoot; 
            }
        }

        ICollection IDictionary.Keys => this.Keys;

        ICollection IDictionary.Values => this.Values;

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => this.Keys;

        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => this.Values;

        ICollection<TKey> IDictionary<TKey, TValue>.Keys => this.Keys;

        ICollection<TValue> IDictionary<TKey, TValue>.Values => this.Values;

        private List<KeyValuePair<TKey, TValue>> List => this.list;

        private Dictionary<TKey, int> Dictionary => this.dictionary;

        private IEqualityComparer<TValue> ValueComparer => Comparers.EqualityComparer<TValue>.Default;

        public IEqualityComparer<TKey> Comparer => this.comparer;

        public int Count
            => this.List.Count;

        public bool IsReadOnly => false;

        public KeyCollection Keys
            => this.keys ??= new KeyCollection(this);

        public ValueCollection Values
            => this.values ??= new ValueCollection(this);
#endregion

#region Indexers
        KeyValuePair<TKey, TValue> IOrderedDictionary<TKey, TValue>.this[int index]
        {
            get => this.List[index];
            set => this.InsertAt(index, value, isAdding: false);
        }

        object System.Collections.Specialized.IOrderedDictionary.this[int index]
        {
            get
            {
                var pair = ((IOrderedDictionary<TKey, TValue>) this)[index];
                return new DictionaryEntry(pair.Key, pair.Value);
            }
            set
            {
                var entry = ThrowHelper.CastArgumentOrThrow<DictionaryEntry>(value, nameof(value));
                var pair = new KeyValuePair<TKey, TValue>((TKey) entry.Key, (TValue) entry.Value);
                ((IOrderedDictionary<TKey, TValue>) this)[index] = pair;
            }
        }

        object IDictionary.this[object key]
        {
            get => this[ThrowHelper.CastArgumentOrThrow<TKey>(key, nameof(key))];
            set
            {
                var keyT = ThrowHelper.CastArgumentOrThrow<TKey>(key, nameof(key));
                var valueT = ThrowHelper.CastArgumentOrThrow<TValue>(value, nameof(value));
                this[keyT] = valueT;
            }
        }

        public TValue this[TKey key]
        {
            get => this.List[this.Dictionary[key]].Value;
            set => this.Insert(key, value, isAdding: false);
        }
#endregion

#region Constructors
        private OrderedDictionary(
            List<KeyValuePair<TKey, TValue>> list,
            Dictionary<TKey, int> dictionary,
            IEqualityComparer<TKey> comparer)
        {
            this.list = list ?? throw new ArgumentNullException(nameof(list));
            this.dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
            this.comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
        }

        protected OrderedDictionary(SerializationInfo info, StreamingContext context)
        {
            serializationInfo = info;
            streamingContext = context;
        }

        public OrderedDictionary() :
            this(Comparers.EqualityComparer<TKey>.Default)
        {
        }
        
        public OrderedDictionary(IDictionary<TKey, TValue> dictionary) :
            this(dictionary, GetComparer(dictionary) ?? Comparers.EqualityComparer<TKey>.Default)
        {
        }

        public OrderedDictionary(IEqualityComparer<TKey> comparer) :
            this(
                new List<KeyValuePair<TKey, TValue>>(),
                new Dictionary<TKey, int>(comparer),
                comparer)
        {
        }

        public OrderedDictionary(int capacity) :
            this(capacity, Comparers.EqualityComparer<TKey>.Default)
        {
        }
        
        public OrderedDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer) :
            this(dictionary.Count, comparer)
        {
            foreach (var item in dictionary)
            {
                this.Add(item);
            }
        }

        public OrderedDictionary(int capacity, IEqualityComparer<TKey> comparer) :
            this(
                new List<KeyValuePair<TKey, TValue>>(capacity),
                new Dictionary<TKey, int>(capacity, comparer),
                comparer)
        {
        }
#endregion

        private static IEqualityComparer<TKey> GetComparer(IDictionary<TKey, TValue> dictionary)
        {
            if (dictionary is OrderedDictionary<TKey, TValue> od)
                return od.Comparer;

            if (dictionary is Dictionary<TKey, TValue> d)
                return Comparers.EqualityComparer.From(d.Comparer);

            return null;
        }

#region Methods
        void System.Collections.Specialized.IOrderedDictionary.Insert(int index, object key, object value)
        {
            this.InsertAt(
                index,
                ThrowHelper.CastArgumentOrThrow<TKey>(key, nameof(key)),
                ThrowHelper.CastArgumentOrThrow<TValue>(value, nameof(value)));
        }

        void IDictionary.Add(object key, object value)
        {
            this.Add(
                ThrowHelper.CastArgumentOrThrow<TKey>(key, nameof(key)),
                ThrowHelper.CastArgumentOrThrow<TValue>(value, nameof(value)));
        }

        bool IDictionary.Contains(object key)
            => this.ContainsKey(ThrowHelper.CastArgumentOrThrow<TKey>(key, nameof(key)));

        IDictionaryEnumerator System.Collections.Specialized.IOrderedDictionary.GetEnumerator()
            => DictionaryEnumerator.Create(this.GetEnumerator(), DictionaryEnumerator.ReturnBehaviors.DictionaryEntry);

        IDictionaryEnumerator IDictionary.GetEnumerator()
            => ((System.Collections.Specialized.IOrderedDictionary) this).GetEnumerator();

        void IDictionary.Remove(object key)
            => this.Remove(ThrowHelper.CastArgumentOrThrow<TKey>(key, nameof(key)));

        void ICollection.CopyTo(Array array, int index)
            => ((ICollection) this.List).CopyTo(array, index);

        IEnumerator IEnumerable.GetEnumerator()
            => this.GetEnumerator();

        void IDeserializationCallback.OnDeserialization(object sender)
            => this.OnDeserialization(sender);

        private void RebuildDictionary(int startIndex = 0)
        {
            for (int i = startIndex; i < this.List.Count; i++)
            {
                var item = this.List[i];
                this.Dictionary[item.Key] = i;
            }
        }

        private void Insert(TKey key, TValue value, bool isAdding)
            => this.Insert(new KeyValuePair<TKey, TValue>(key, value), isAdding);

        private void Insert(KeyValuePair<TKey, TValue> item, bool isAdding)
        {
            if (isAdding)
            {
                this.Dictionary.Add(item.Key, this.List.Count);
                this.List.Add(item);
            }
            else
            {
                if (this.Dictionary.TryGetValue(item.Key, out int index))
                {
                    this.Dictionary.Remove(item.Key);
                    this.List.RemoveAt(index);
                    this.RebuildDictionary(index);
                }
                
                this.Insert(item, isAdding: true);
            }
        }

        private void InsertAt(int index, TKey key, TValue value, bool isAdding)
            => this.InsertAt(index, new KeyValuePair<TKey, TValue>(key, value), isAdding);

        private void InsertAt(int index, KeyValuePair<TKey, TValue> item, bool isAdding)
        {
            if (isAdding)
            {
                this.Dictionary.Add(item.Key, index);

                try
                {
                    this.List.Insert(index, item);
                }
                catch (Exception)
                {
                    this.Dictionary.Remove(item.Key);
                    throw;
                }

                this.RebuildDictionary(index + 1);
            }
            else
            {
                if (this.Dictionary.TryGetValue(item.Key, out int currentIndex))
                {
                    var currentItem = this.List[currentIndex];
                    this.Dictionary.Remove(currentItem.Key);
                    this.List.RemoveAt(currentIndex);

                    try
                    {
                        this.InsertAt(index, item, isAdding: true);
                    }
                    catch (Exception)
                    {
                        this.InsertAt(currentIndex, currentItem, isAdding: true);
                        throw;
                    }
                }
                else
                {
                    this.InsertAt(index, item, isAdding: true);
                }
            }
        }

        protected virtual void OnDeserialization(object sender)
        {
            this.list = (List<KeyValuePair<TKey, TValue>>) this.serializationInfo.GetValue(
                nameof(OrderedDictionary<TKey, TValue>.List),
                typeof(List<KeyValuePair<TKey, TValue>>));

            var comparer = (IEqualityComparer<TKey>) (this.serializationInfo.GetValue(
                nameof(OrderedDictionary<TKey, TValue>.Comparer),
                typeof(IEqualityComparer<TKey>)) ?? Comparers.EqualityComparer<TKey>.Default);

            this.dictionary = new Dictionary<TKey, int>(this.list.Count, comparer);
            this.comparer = comparer;

            this.RebuildDictionary();
        }

        public bool ContainsKey(TKey key)
            => this.Dictionary.ContainsKey(key);

        public bool ContainsValue(TValue value)
        {
            foreach (var item in this.List)
            {
                if (this.ValueComparer.Equals(value, item.Value))
                    return true;
            }

            return false;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (this.Dictionary.TryGetValue(key, out int index))
            {
                value = this.List[index].Value;
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            if (this.Dictionary.TryGetValue(item.Key, out int index))
            {
                return this.ValueComparer.Equals(item.Value, this.List[index].Value);
            }

            return false;
        }

        public void Add(TKey key, TValue value)
            => this.Add(new KeyValuePair<TKey, TValue>(key, value));

        public void Add(KeyValuePair<TKey, TValue> item)
            => this.Insert(item, isAdding: true);

        public void InsertAt(int index, TKey key, TValue value)
            => this.InsertAt(index, new KeyValuePair<TKey, TValue>(key, value));

        public void InsertAt(int index, KeyValuePair<TKey, TValue> item)
            => this.InsertAt(index, item, isAdding: true);

        public bool Remove(TKey key)
        {
            if (this.Dictionary.TryGetValue(key, out int index))
            {
                this.Dictionary.Remove(key);
                this.List.RemoveAt(index);
                return true;
            }

            return false;
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if (this.Dictionary.TryGetValue(item.Key, out int index))
            {
                if (this.ValueComparer.Equals(item.Value, this.List[index].Value))
                {
                    this.Dictionary.Remove(item.Key);
                    this.List.RemoveAt(index);
                    return true;
                }
            }

            return false;
        }

        public void RemoveAt(int index)
        {
            var item = this.List[index];
            this.Dictionary.Remove(item.Key);
            this.List.RemoveAt(index);
        }

        public void Clear()
        {
            this.Dictionary.Clear();
            this.List.Clear();
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
            => this.List.CopyTo(array, arrayIndex);

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (var item in this.List)
                yield return item;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(OrderedDictionary<TKey, TValue>.List), this.List);
            info.AddValue(
                nameof(OrderedDictionary<TKey, TValue>.Comparer),
                object.ReferenceEquals(this.Comparer, Comparers.EqualityComparer<TKey>.Default) ? this.Comparer : null);
        }
#endregion
    }
}
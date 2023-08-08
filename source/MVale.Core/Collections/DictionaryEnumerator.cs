using System;
using System.Collections;
using System.Collections.Generic;

namespace MVale.Core.Collections
{
    public static class DictionaryEnumerator
    {
        public enum ReturnBehaviors
        {
            KeyValuePair = 0,
            DictionaryEntry
        }

        [Serializable]
        public sealed class Adapter<TKey, TValue> : DictionaryEnumerator<TKey, TValue>, IEnumerator
        {
            object IEnumerator.Current
            {
                get
                {
                    switch (this.ReturnBehavior)
                    {
                        case ReturnBehaviors.KeyValuePair:
                            return this.Current;

                        case ReturnBehaviors.DictionaryEntry:
                            return ((IDictionaryEnumerator) this).Current;

                        default:
                            throw new InvalidOperationException();
                    }
                }
            }

            public IEnumerator<KeyValuePair<TKey, TValue>> Enumerator { get; }

            public ReturnBehaviors ReturnBehavior { get; } = DefaultReturnBehavior;

            public override KeyValuePair<TKey, TValue> Current => this.Enumerator.Current;

            public Adapter(IEnumerator<KeyValuePair<TKey, TValue>> enumerator)
            {
                Enumerator = enumerator ?? throw new ArgumentNullException(nameof(enumerator));
            }

            public Adapter(
                IEnumerator<KeyValuePair<TKey, TValue>> enumerator,
                ReturnBehaviors returnBehavior)
            {
                if (enumerator == null)
                    throw new ArgumentNullException(nameof(enumerator));

                ThrowHelper.ThrowIfUndefinedEnumArgument(returnBehavior, nameof(returnBehavior));

                Enumerator = enumerator;
                ReturnBehavior = returnBehavior;
            }

            public override void Dispose()
                => this.Enumerator.Dispose();

            public override bool MoveNext()
                => this.Enumerator.MoveNext();

            public override void Reset()
                => this.Enumerator.Reset();
        }

        public const ReturnBehaviors DefaultReturnBehavior = ReturnBehaviors.KeyValuePair;

        public static DictionaryEnumerator<TKey, TValue> Create<TKey, TValue>(
            IEnumerator<KeyValuePair<TKey, TValue>> enumerator,
            ReturnBehaviors returnBehavior = DefaultReturnBehavior)
            => new Adapter<TKey, TValue>(enumerator, returnBehavior);
    }
}
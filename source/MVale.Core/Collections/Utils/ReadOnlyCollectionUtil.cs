using System;
using System.Collections.Generic;

namespace MVale.Core.Collections.Utils
{
    internal static class ReadOnlyCollectionUtil
    {
        public static void CopyTo<T>(IReadOnlyCollection<T> instance, T[] array, int arrayIndex)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            if (array == null)
                throw new ArgumentNullException(nameof(array));

            if (arrayIndex < 0 || arrayIndex > array.Length)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), arrayIndex, (string) null);

            if (array.Length - arrayIndex < instance.Count)
                throw new ArgumentException((string) null, nameof(array));
            
            int count = instance.Count;
            
            foreach (var item in instance)
            {
                array[arrayIndex++] = item;
            }
        }

        public static void CopyTo<T>(IReadOnlyCollection<T> instance, Array array, int index)
        {
            CopyTo(instance, ThrowHelper.CastArgumentOrThrow<T[]>(array, nameof(array)), index);
        }
    }
}
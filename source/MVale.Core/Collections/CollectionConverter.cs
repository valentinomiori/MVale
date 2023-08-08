#if NETCOREAPP
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace CashPoint.Core.Serialization
{
    public class CollectionConverter
    {
        public interface IConverter
        {
            bool CanConvert(Type collectionType, out Type elementType);
            object Create(Type collectionType, int? count, IEnumerable<object> elements);
            IEnumerable<object> GetElements(object collection, Type collectionType, out int? count);
        }

        class ArrayConverter : IConverter
        {
            public bool CanConvert(Type collectionType, out Type elementType)
            {
                if (collectionType.IsArray && (collectionType.GetArrayRank() == 1))
                {
                    elementType = collectionType.GetElementType();
                    return true;
                }

                elementType = null;
                return false;
            }

            public object Create(Type collectionType, int? count, IEnumerable<object> elements)
            {
                var elementType = collectionType.GetElementType();
                Array array = null;

                if (count.HasValue)
                {
                    array = Array.CreateInstance(elementType, count.Value);

                    int index = 0;
                    foreach (var element in elements)
                    {
                        array.SetValue(element, index++);
                    }
                }
                else
                {
                    var list = elements.ToImmutableList();
                    array = (Array) this.Create(collectionType, list.Count, list);
                }

                return array;
            }

            public IEnumerable<object> GetElements(object collection, Type collectionType, out int? count)
            {
                if (!this.CanConvert(collectionType, out var elementType))
                    throw new InvalidOperationException();

                count = ((Array) collection).Length;
                return Enumerable.Cast<object>((IEnumerable) collection);
            }
        }

        class ListConverter : IConverter
        {
            public bool CanConvert(Type collectionType, out Type elementType)
            {
                if (!collectionType.IsGenericType)
                {
                    elementType = null;
                    return false;
                }

                if (collectionType.GetGenericTypeDefinition() != typeof(List<>))
                {
                    elementType = null;
                    return false;
                }

                elementType = collectionType.GetGenericArguments()[0];
                return true;
            }

            public object Create(Type collectionType, int? count, IEnumerable<object> elements)
            {
                var list
                    = (IList) (count.HasValue
                    ? Activator.CreateInstance(collectionType, count.Value)
                    : Activator.CreateInstance(collectionType));

                foreach (var element in elements)
                {
                    list.Add(element);
                }

                return list;
            }

            public IEnumerable<object> GetElements(object collection, Type collectionType, out int? count)
            {
                if (!this.CanConvert(collectionType, out var elementType))
                    throw new InvalidOperationException();

                count = ((IList) collection).Count;
                return Enumerable.Cast<object>((IEnumerable) collection);
            }
        }

        class LinkedListConverter : IConverter
        {
            public bool CanConvert(Type collectionType, out Type elementType)
            {
                if (!collectionType.IsGenericType)
                {
                    elementType = null;
                    return false;
                }

                if (collectionType.GetGenericTypeDefinition() != typeof(LinkedList<>))
                {
                    elementType = null;
                    return false;
                }

                elementType = collectionType.GetGenericArguments()[0];
                return true;
            }

            public object Create(Type collectionType, int? count, IEnumerable<object> elements)
            {
                var linkedList = (ICollection) Activator.CreateInstance(collectionType);

                foreach (var element in elements)
                {
                    ((dynamic) linkedList).AddLast(element);
                }

                return linkedList;
            }

            public IEnumerable<object> GetElements(object collection, Type collectionType, out int? count)
            {
                if (!this.CanConvert(collectionType, out var elementType))
                    throw new InvalidOperationException();

                count = ((ICollection) collection).Count;
                return Enumerable.Cast<object>((IEnumerable) collection);
            }
        }

        class QueueConverter : IConverter
        {
            public bool CanConvert(Type collectionType, out Type elementType)
            {
                if (!collectionType.IsGenericType)
                {
                    elementType = null;
                    return false;
                }

                if (collectionType.GetGenericTypeDefinition() != typeof(Queue<>))
                {
                    elementType = null;
                    return false;
                }

                elementType = collectionType.GetGenericArguments()[0];
                return true;
            }

            public object Create(Type collectionType, int? count, IEnumerable<object> elements)
            {
                dynamic queue
                    = count.HasValue
                    ? Activator.CreateInstance(collectionType, count.Value)
                    : Activator.CreateInstance(collectionType);

                foreach (var element in elements)
                {
                    queue.Enqueue((dynamic) element);
                }

                return queue;
            }

            public IEnumerable<object> GetElements(object collection, Type collectionType, out int? count)
            {
                if (!this.CanConvert(collectionType, out var elementType))
                    throw new InvalidOperationException();

                count = ((ICollection) collection).Count;
                return Enumerable.Cast<object>((IEnumerable) collection);
            }
        }

        class StackConverter : IConverter
        {
            public bool CanConvert(Type collectionType, out Type elementType)
            {
                if (!collectionType.IsGenericType)
                {
                    elementType = null;
                    return false;
                }

                if (collectionType.GetGenericTypeDefinition() != typeof(Stack<>))
                {
                    elementType = null;
                    return false;
                }

                elementType = collectionType.GetGenericArguments()[0];
                return true;
            }

            public object Create(Type collectionType, int? count, IEnumerable<object> elements)
            {
                var stack
                    = (ICollection) (count.HasValue
                    ? Activator.CreateInstance(collectionType, count.Value)
                    : Activator.CreateInstance(collectionType));

                foreach (var element in elements.Reverse())
                {
                    ((dynamic) stack).Push((dynamic) element);
                }

                return stack;
            }

            public IEnumerable<object> GetElements(object collection, Type collectionType, out int? count)
            {
                if (!this.CanConvert(collectionType, out var elementType))
                    throw new InvalidOperationException();

                count = ((ICollection) collection).Count;
                return Enumerable.Cast<object>((IEnumerable) collection);
            }
        }

        class DictionaryConverter : IConverter
        {
            static IEnumerable<object> Enumerate<TKey, TValue>(IDictionary<TKey, TValue> dictionary)
            {
                return Enumerable.Select(dictionary, pair => (object) pair);
            }

            public bool CanConvert(Type collectionType, out Type elementType)
            {
                if (!collectionType.IsGenericType)
                {
                    elementType = null;
                    return false;
                }

                if (collectionType.GetGenericTypeDefinition() != typeof(Dictionary<,>))
                {
                    elementType = null;
                    return false;
                }

                var genericEnumerableType = collectionType.GetInterfaces()
                .Where(i => i.IsGenericType)
                .Single(i => i.GetGenericTypeDefinition() == typeof(IEnumerable<>));

                elementType = genericEnumerableType.GetGenericArguments()[0];
                return true;
            }

            public object Create(Type collectionType, int? count, IEnumerable<object> elements)
            {
                IDictionary dictionary = null;
                if (count.HasValue)
                {
                    dictionary = (IDictionary) Activator.CreateInstance(collectionType, new object[] { count.Value });
                }
                else
                {
                    dictionary = (IDictionary) Activator.CreateInstance(collectionType, new object[0]);
                }

                foreach (var element in elements)
                {
                    dictionary.Add(((dynamic) element).Key, ((dynamic) element).Value);
                }

                return dictionary;
            }

            public IEnumerable<object> GetElements(object collection, Type collectionType, out int? count)
            {
                if (!this.CanConvert(collectionType, out var elementType))
                    throw new InvalidOperationException();

                count = ((IDictionary) collection).Count;
                return Enumerate((dynamic) collection);
            }
        }

        class ImmutableArrayConverter : IConverter
        {
            private static readonly MethodInfo CreateBuilderMethod
                = typeof(ImmutableArray)
                    .GetMethods()
                    .Where(m => m.IsGenericMethodDefinition && (m.GetGenericArguments().Length == 1))
                    .Where(m => m.Name == nameof(ImmutableArray.CreateBuilder))
                    .Single(m => m.GetParameters().Length == 0);

            private static readonly MethodInfo CreateBuilderMethodWithInitialCapacity
                = typeof(ImmutableArray)
                    .GetMethods()
                    .Where(m => m.IsGenericMethodDefinition && (m.GetGenericArguments().Length == 1))
                    .Where(m => m.Name == nameof(ImmutableArray.CreateBuilder))
                    .Single(m => m.GetParameters().Length == 1 && m.GetParameters()[0].ParameterType == typeof(int));

            public bool CanConvert(Type collectionType, out Type elementType)
            {
                if (!collectionType.IsGenericType)
                {
                    elementType = null;
                    return false;
                }

                if (collectionType.GetGenericTypeDefinition() != typeof(ImmutableArray<>))
                {
                    elementType = null;
                    return false;
                }

                if (collectionType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    return this.CanConvert(collectionType.GetGenericArguments()[0], out elementType);
                }

                elementType = collectionType.GetGenericArguments()[0];
                return true;
            }

            public object Create(Type collectionType, int? count, IEnumerable<object> elements)
            {
                if (!this.CanConvert(collectionType, out var elementType))
                    throw new InvalidOperationException();

                dynamic builder = null;
                if (count.HasValue)
                {
                    builder = CreateBuilderMethodWithInitialCapacity.MakeGenericMethod(new[] { elementType })
                    .Invoke(null, new object[] { count.Value });
                }
                else
                {
                    builder = CreateBuilderMethod.MakeGenericMethod(new[] { elementType })
                    .Invoke(null, new object[0]);
                }

                foreach (var element in elements)
                {
                    builder.Add((dynamic) element);
                }

                return builder.MoveToImmutable();
            }

            public IEnumerable<object> GetElements(object collection, Type collectionType, out int? count)
            {
                if (!this.CanConvert(collectionType, out var elementType))
                    throw new InvalidOperationException();

                count = ((ICollection) collection).Count;
                return Enumerable.Cast<object>((IEnumerable) collection);
            }
        }

        class ImmutableListConverter : IConverter
        {
            private static readonly MethodInfo CreateBuilderMethod
                = typeof(ImmutableList).GetMethods()
                .Where(m => m.IsGenericMethodDefinition && (m.GetGenericArguments().Length == 1))
                .Where(m => m.Name == nameof(ImmutableList.CreateBuilder))
                .Single(m => m.GetParameters().Length == 0);

            public bool CanConvert(Type collectionType, out Type elementType)
            {
                if (!collectionType.IsGenericType)
                {
                    elementType = null;
                    return false;
                }

                if (collectionType.GetGenericTypeDefinition() != typeof(ImmutableList<>))
                {
                    elementType = null;
                    return false;
                }

                elementType = collectionType.GetGenericArguments()[0];
                return true;
            }

            public object Create(Type collectionType, int? count, IEnumerable<object> elements)
            {
                if (!this.CanConvert(collectionType, out var elementType))
                    throw new InvalidOperationException();

                dynamic builder = CreateBuilderMethod.MakeGenericMethod(new[] { elementType })
                .Invoke(null, new object[0]);

                foreach (var element in elements)
                {
                    builder.Add((dynamic) element);
                }

                return builder.ToImmutable();
            }

            public IEnumerable<object> GetElements(object collection, Type collectionType, out int? count)
            {
                if (!this.CanConvert(collectionType, out var elementType))
                    throw new InvalidOperationException();

                count = ((ICollection) collection).Count;
                return Enumerable.Cast<object>((IEnumerable) collection);
            }
        }

        class ImmutableQueueConverter : IConverter
        {
            public bool CanConvert(Type collectionType, out Type elementType)
            {
                if (!collectionType.IsGenericType)
                {
                    elementType = null;
                    return false;
                }

                if (collectionType.GetGenericTypeDefinition() != typeof(ImmutableQueue<>))
                {
                    elementType = null;
                    return false;
                }

                elementType = collectionType.GetGenericArguments()[0];
                return true;
            }

            public object Create(Type collectionType, int? count, IEnumerable<object> elements)
            {
                if (!this.CanConvert(collectionType, out var elementType))
                    throw new InvalidOperationException();

                throw new NotImplementedException();//TODO
            }

            public IEnumerable<object> GetElements(object collection, Type collectionType, out int? count)
            {
                if (!this.CanConvert(collectionType, out var elementType))
                    throw new InvalidOperationException();

                count = null;
                return Enumerable.Cast<object>((IEnumerable) collection);
            }
        }

        public static readonly CollectionConverter Default
            = new CollectionConverter(ImmutableList.Create<IConverter>(
                new ArrayConverter(),
                new ListConverter(),
                new LinkedListConverter(),
                new QueueConverter(),
                new StackConverter(),
                new DictionaryConverter(),
                new ImmutableArrayConverter(),
                new ImmutableListConverter()));

        private ImmutableList<IConverter> Converters { get; }

        private ConcurrentDictionary<(Type, Type), (IConverter, IConverter)?> Conversions { get; }
            = new ConcurrentDictionary<(Type, Type), (IConverter, IConverter)?>();

        public CollectionConverter(ImmutableList<IConverter> converters)
        {
            this.Converters = converters ?? throw new ArgumentNullException(nameof(converters));
        }

        private (IConverter, IConverter)? ConversionValueFactory((Type, Type) collectionTypePairs)
            => this.FindConversion(collectionTypePairs.Item1, collectionTypePairs.Item2);

        private (IConverter, IConverter)? FindConversion(Type sourceCollectionType, Type destinationCollectionType)
        {
            foreach (var sourceConverter in this.Converters)
            {
                if (sourceConverter.CanConvert(sourceCollectionType, out var elementType))
                {
                    foreach (var destinationConverter in this.Converters)
                    {
                        if (destinationConverter.CanConvert(destinationCollectionType, out var destinationElementType))
                        {
                            if (elementType == destinationElementType)
                            {
                                return (sourceConverter, destinationConverter);
                            }
                        }
                    }
                }
            }

            return null;
        }

        private (IConverter, IConverter)? GetConversion(Type sourceCollectionType, Type destinationCollectionType)
        {
            var key = (sourceCollectionType, destinationCollectionType);
            return this.Conversions.GetOrAdd(key, this.ConversionValueFactory);
        }
        
        public bool CanConvert(Type sourceCollectionType, Type destinationCollectionType)
        {
            if (sourceCollectionType == null)
                throw new ArgumentNullException(nameof(sourceCollectionType));

            if (destinationCollectionType == null)
                throw new ArgumentNullException(nameof(destinationCollectionType));

            var conversion = this.GetConversion(sourceCollectionType, destinationCollectionType);
            return conversion != null;
        }

        public object Convert(object collection, Type sourceCollectionType, Type destinationCollectionType)
        {
            if (sourceCollectionType == null)
                throw new ArgumentNullException(nameof(sourceCollectionType));

            if (destinationCollectionType == null)
                throw new ArgumentNullException(nameof(destinationCollectionType));

            var conversion = this.GetConversion(sourceCollectionType, destinationCollectionType);

            if (!conversion.HasValue)
                throw new NotSupportedException(
                    $"Collection type: {sourceCollectionType} is not convertible to: {destinationCollectionType}.");

            var elements = conversion.Value.Item1.GetElements(collection, sourceCollectionType, out int? count);
            return conversion.Value.Item2.Create(destinationCollectionType, count, elements);
        }
    }
}
#endif
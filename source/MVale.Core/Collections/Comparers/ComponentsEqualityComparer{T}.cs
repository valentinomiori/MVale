using System;
using System.Collections.Generic;
using System.Linq;

namespace MVale.Core.Collections.Comparers
{
    [Serializable]
    public abstract class ComponentsEqualityComparer<T> : EqualityComparer<T>
    {
        public interface IComponent
        {
            bool CompareComponent(T x, T y);
            int GetComponentHashCode(T obj);
        }

        public sealed class Component<TComponent> : IComponent
        {
            public Func<T, TComponent> Getter { get; }

            public IEqualityComparer<TComponent> Comparer { get; }

            public Component(Func<T, TComponent> getter) :
                this(getter, EqualityComparer<TComponent>.Default)
            {
            }

            public Component(Func<T, TComponent> getter, IEqualityComparer<TComponent> comparer)
            {
                Getter = getter ?? throw new ArgumentNullException(nameof(getter));
                Comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
            }

            public bool CompareComponent(T x, T y)
            {
                return this.Comparer.Equals(this.Getter(x), this.Getter(y));
            }

            public int GetComponentHashCode(T obj)
            {
                return this.Comparer.GetHashCode(this.Getter(obj));
            }
        }

        protected IEnumerable<IComponent> Components => this.GetComponents();

        public ComponentsEqualityComparer()
        {
        }

        public static Component<TComponent> CreateComponent<TComponent>(
            Func<T, TComponent> getter,
            IEqualityComparer<TComponent> comparer = null)
        {
            return new Component<TComponent>(getter, comparer ?? EqualityComparer<TComponent>.Default);
        }

        protected abstract IEnumerable<IComponent> GetComponents();

        public override bool Equals(T x, T y)
        {
            foreach (var component in this.Components)
            {
                if (!component.CompareComponent(x, y))
                    return false;
            }

            return true;
        }

        public override int GetHashCode(T obj)
        {
            return InternalHashCodeUtil.CombineRange(this.Components.Select(c => c.GetComponentHashCode(obj)));
        }
    }
}
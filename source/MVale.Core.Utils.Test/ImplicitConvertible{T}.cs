using System;

namespace MVale.Core.Utils.Test
{
    class ImplicitConvertible<T>
    {
        public T Value { get; set; }

        public static implicit operator T(ImplicitConvertible<T> self) => self.Value;
    }
}
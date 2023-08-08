using System;

namespace MVale.Core.Utils.Test
{
    class ExplicitConvertible<T>
    {
        public T Value { get; set; }

        public static explicit operator T(ExplicitConvertible<T> self) => self.Value;
    }
}
using System;
using System.ComponentModel;

namespace MVale.Core.Collections
{
    internal static class ThrowHelper
    {
        public static void ThrowIfUndefinedEnumArgument<T>(T value, string argumentName)
            where T : Enum
        {
            int intValue = checked((int) (dynamic) value);

            if (!Enum.IsDefined(typeof(T), value))
                throw new InvalidEnumArgumentException(argumentName, intValue, typeof(T));
        }

        public static T CastArgumentOrThrow<T>(object obj, string paramName)
        {
            T value;

            try
            {
                value = (T) obj;
            }
            catch (InvalidCastException exception)
            {
                throw new ArgumentException((string) null, paramName, exception);
            }

            return value;
        }
    }
}
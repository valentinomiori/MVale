using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace MVale.Core.Utils
{
    /// <summary>
    /// Utility class to perform casts.
    /// </summary>
    public static class CastUtil
    {
        private static ConditionalWeakTable<Type, ConditionalWeakTable<Type, object>> Casts { get; }
            = new ConditionalWeakTable<Type, ConditionalWeakTable<Type, object>>();

        private static Func<TValue, TResult> CreateCastDelegate<TValue, TResult>()
        {
            var parameter = Expression.Parameter(typeof(TValue));
            var body = Expression.Convert(parameter, typeof(TResult));
            return Expression.Lambda<Func<TValue, TResult>>(body, parameter).Compile();
        }

        /// <summary>
        /// Get or create a delegate to cast from <typeparamref name="TValue"/> to <typeparamref name="TResult"/>.
        /// </summary>
        /// <typeparam name="TValue">Delegate parameter type.</typeparam>
        /// <typeparam name="TResult">Delegate return type.</typeparam>
        /// <returns>A delegate that can cast the parameter.</returns>
        public static Func<TValue, TResult> GetCastDelegate<TValue, TResult>()
        {
            if (Casts == null)
                return CreateCastDelegate<TValue, TResult>();

            var table = Casts.GetValue(typeof(TValue), t => new ConditionalWeakTable<Type, object>());
            return (Func<TValue, TResult>) table.GetValue(typeof(TResult), t => CreateCastDelegate<TValue, TResult>());
        }

        /// <summary>
        /// Execute the cast of <paramref name="value"/> from <typeparamref name="TValue"/> to <typeparamref name="TResult"/>.
        /// </summary>
        /// <typeparam name="TValue">The parameter type.</typeparam>
        /// <typeparam name="TResult">The return type.</typeparam>
        /// <param name="value">The value to cast.</param>
        /// <returns>The casted value.</returns>
        public static TResult Cast<TValue, TResult>(TValue value)
        {
            return GetCastDelegate<TValue, TResult>().Invoke(value);
        }
    }
}
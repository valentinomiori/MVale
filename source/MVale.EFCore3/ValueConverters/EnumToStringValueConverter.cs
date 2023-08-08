using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MVale.EntityFramework.ValueConverters
{
    public sealed class EnumToStringValueConverter<T> : ValueConverter<T, string>
        where T : struct, Enum
    {
        public static Expression<Func<T, string>> EncodingExpression { get; }
            = e => e.ToString();

        public static Expression<Func<string, T>> DecodingExpression { get; }
            = s => (T) Enum.Parse(typeof(T), s);

        public EnumToStringValueConverter() : base(EncodingExpression, DecodingExpression)
        {
        }
    }
}
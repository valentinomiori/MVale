using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MVale.EntityFramework.ValueConverters
{
    public sealed class NullableEnumToStringValueConverter<T> : ValueConverter<T?, string>
        where T : struct, Enum
    {
        public static Expression<Func<T?, string>> EncodingExpression { get; }
            = e => e != null ? e.ToString() : null;

        public static Expression<Func<string, T?>> DecodingExpression { get; }
            = s => s != null ? (T?) Enum.Parse(typeof(T), s) : null;

        public NullableEnumToStringValueConverter() : base(EncodingExpression, DecodingExpression)
        {
        }
    }
}
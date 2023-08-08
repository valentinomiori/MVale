using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MVale.EntityFramework.ValueConverters;

namespace MVale.EntityFramework.ModelBuilderActions
{
#if EntityFrameworkCore3
    /// <summary>
    /// Sets an instance of <see cref="ValueConverters.EnumToStringValueConverter{T}"/> or
    /// <see cref="ValueConverters.NullableEnumToStringValueConverter{T}"/> as the
    /// <see cref="ValueConverter"/> for every property of every entity in the model that has
    /// <see cref="IPropertyBase.ClrType"/> with <see cref="Type.IsEnum"/> as <see langword="true"/>
    /// (or the type parameter of <see cref="Nullable{T}"/>).
    /// </summary>
#else
    /// <summary>
    /// Sets an instance of <see cref="ValueConverters.EnumToStringValueConverter{T}"/> or
    /// <see cref="ValueConverters.NullableEnumToStringValueConverter{T}"/> as the
    /// <see cref="ValueConverter"/> for every property of every entity in the model that has
    /// <see cref="IReadOnlyPropertyBase.ClrType"/> with <see cref="Type.IsEnum"/> as <see langword="true"/>
    /// (or the type parameter of <see cref="Nullable{T}"/>).
    /// </summary>
#endif
    public sealed class EnumToStringConversions : IModelBuilderAction
    {
        public static EnumToStringConversions Instance { get; }
            = new EnumToStringConversions();

        private EnumToStringConversions()
        {
        }

        public void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach(var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType.IsEnum)
                    {
                        var converterType = typeof(EnumToStringValueConverter<>)
                        .MakeGenericType(new[] { property.ClrType });

                        property.SetValueConverter((ValueConverter) Activator.CreateInstance(converterType));
                    }
                    else
                    {
                        var underlyingType = Nullable.GetUnderlyingType(property.ClrType);

                        if (underlyingType?.IsEnum ?? false)
                        {
                            var converterType = typeof(NullableEnumToStringValueConverter<>)
                            .MakeGenericType(new[] { underlyingType });

                            property.SetValueConverter((ValueConverter) Activator.CreateInstance(converterType));
                        }
                    }
                }
            }
        }
    }
}
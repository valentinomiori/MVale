#if NETCOREAPP
using System;
using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MVale.Core.Serialization.Json
{
    public sealed class DynamicJsonConverter : JsonConverter<object>
    {
        public Type BaseType { get; }

        public ImmutableList<Type> DerivedTypes { get; }

        public DynamicJsonConverter(
            Type baseType,
            ImmutableList<Type> derivedTypes)
        {
            if (baseType == null)
                throw new ArgumentNullException(nameof(baseType));

            if (derivedTypes != null)
            {
                foreach (var t in derivedTypes)
                {
                    if (!baseType.IsAssignableFrom(t))
                        throw new ArgumentException(
                            $"The type '{t}' is not assignable to type '{baseType}'.",
                            nameof(derivedTypes));
                }
            }

            BaseType = baseType;
            DerivedTypes = derivedTypes;
        }
        
        public override bool CanConvert(Type typeToConvert)
        {
            return this.BaseType.IsAssignableFrom(typeToConvert);
        }

        public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return JsonSerializer.Deserialize(ref reader, this.BaseType, options);
        }

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            if (value != null && (this.DerivedTypes == null || this.DerivedTypes.Contains(value.GetType())))
            {
                JsonSerializer.Serialize(writer, value, value.GetType(), options);
            }

            JsonSerializer.Serialize<object>(writer, value, options);
        }
    }
}
#endif
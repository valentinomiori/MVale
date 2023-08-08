#if NETCOREAPP
using System;
using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MVale.Core.Serialization.Json
{
    public sealed class DynamicJsonConverter : JsonConverter<object>
    {
        public Type Type { get; }

        public ImmutableList<Type> DerivedTypes { get; }

        public DynamicJsonConverter(
            Type type,
            ImmutableList<Type> derivedTypes)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (derivedTypes != null)
            {
                foreach (var t in derivedTypes)
                {
                    if (!type.IsAssignableFrom(t))
                        throw new ArgumentException(
                            $"The type {t} is not assignable to type {type}.",
                            nameof(derivedTypes));
                }
            }

            Type = type;
            DerivedTypes = derivedTypes;
        }
        
        public override bool CanConvert(Type typeToConvert)
        {
            return this.Type.IsAssignableFrom(typeToConvert);
        }

        public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return JsonSerializer.Deserialize(ref reader, this.Type, options);
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
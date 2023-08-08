#if NETCOREAPP
using System;
using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MVale.Core.Serialization.Json
{
    public sealed class DynamicJsonConverterAttribute : JsonConverterAttribute
    {
        public Type[] DerivedTypes { get; }

        public DynamicJsonConverterAttribute()
        {
        }

        public DynamicJsonConverterAttribute(params Type[] derivedTypes)
        {
            this.DerivedTypes = derivedTypes;
        }

        public override JsonConverter CreateConverter(Type typeToConvert)
        {
            return new DynamicJsonConverter(typeToConvert, this.DerivedTypes.ToImmutableList());
        }
    }
}
#endif
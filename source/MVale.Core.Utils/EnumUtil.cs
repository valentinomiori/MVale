using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace MVale.Core.Utils
{
    /// <summary>
    /// A utility class regarding enum types, values and names.
    /// Related: <seealso cref="Enum"/>
    /// </summary>
    public static class EnumUtil
    {
        [System.Diagnostics.DebuggerStepThrough]
        internal static void ThrowIfInvalid(Type type, string? paramName = null)
        {
            paramName ??= nameof(type);

            if (type == null)
                throw new ArgumentNullException(paramName);

            if (type == typeof(Enum) || !type.IsEnum)
                throw new ArgumentException(
                    $"Type '{type}' is invalid, it must be a subclass of '{typeof(Enum)}'.", paramName);
        }

        internal static bool IsDefault(Enum @enum)
        {
            return object.Equals(@enum, ReflectionUtil.GetDefaultValue(@enum.GetType()));
        }

        internal static IEnumerable<Enum> InternalGetValues(Type type)
        {
            return Enum.GetValues(type).Cast<Enum>();
        }

        internal static IEnumerable<KeyValuePair<string, Enum>> InternalGetNamedValues(Type type)
        {
            return
            from name in Enum.GetNames(type)
            join value in Enum.GetValues(type).Cast<Enum>()
            on name equals Enum.GetName(type, value)
            select new KeyValuePair<string, Enum>(name, value);
        }

        /// <summary>
        /// Get the values defined for a given enum type.
        /// </summary>
        /// <param name="type">A type representing o subclass of <see cref="Enum"/>.</param>
        /// <returns>A sequence containing the defined enum values.</returns>
        public static IEnumerable<Enum> GetValues(Type type)
        {
            ThrowIfInvalid(type);
            return InternalGetValues(type);
        }

        /// <inheritdoc cref="GetValues(Type)" />
        /// <typeparam name="T">A type representing o subclass of <see cref="Enum"/>.</typeparam>
        public static IEnumerable<T> GetValues<T>()
        where T: struct, Enum
        {
            return InternalGetValues(typeof(T)).Cast<T>();
        }

        /// <summary>
        /// Get the named values defined for a given enum type.
        /// </summary>
        /// <param name="type">A type representing o subclass of <see cref="Enum"/>.</param>
        /// <returns>A sequence containing a pair for each defined enum name as key and the corresponding value.</returns>
        public static IEnumerable<KeyValuePair<string, Enum>> GetNamedValues(Type type)
        {
            ThrowIfInvalid(type);
            return InternalGetNamedValues(type);
        }

        [return: NotNullIfNotNull(nameof(value))]
        public static System.Reflection.FieldInfo? GetField(Enum? value)
        {
            if (value == null)
                return null;

            var type = value.GetType();

            if (Enum.IsDefined(type, value))
            {
                var field = type.GetField(value.ToString());

                if (field != null)
                    return field;
            }

            return type.GetFields()
            .Where(f => f.IsLiteral && f.FieldType == type)
            .First(f => object.Equals(f.GetRawConstantValue(), value));
        }
        
        /// <inheritdoc cref="GetNamedValues(Type)"/>
        /// <typeparam name="T">A type representing o subclass of <see cref="Enum"/>.</typeparam>
        public static IEnumerable<KeyValuePair<string, T>> GetNamedValues<T>()
        where T : struct, Enum
        {
            return InternalGetNamedValues(typeof(T)).Select(p => new KeyValuePair<string, T>(p.Key, (T) p.Value));
        }

        /// <summary>
        /// Check if the type represents a nullable enum.
        /// <para/>
        /// </summary>
        /// <param name="type">A type representing o subclass of <see cref="Enum"/>.</param>
        /// <returns>
        /// <see langword="true"/> if the <paramref name="type"/> parameter is a generic <see cref="Nullable{T}"/>
        /// constructed with an argument for which <see cref="Type.IsEnum"/> is <see langword="true"/>,
        /// or <see langword="false"/> otherwise.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="type"/> is <see langword="null"/></exception>
        public static bool IsNullableEnum(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (!type.IsGenericType)
                return false;

            if (type.GetGenericTypeDefinition() != typeof(Nullable<>))
                return false;

            return type.GetGenericArguments()[0].IsEnum;
        }

        /// <summary>
        /// <inheritdoc cref="GetUnderlyingValue(Enum)"/>
        /// Uses <see cref="GetUnderlyingValue(Enum)"/>.
        /// </summary>
        /// <param name="object">The enum value, or <see langword="null"/>.</param>
        /// <returns><inheritdoc cref="GetUnderlyingValue(Enum)"/></returns>
        /// <exception cref="ArgumentException">If <paramref name="object"/> is not an instance of <see cref="Enum"/>.</exception>
        [return: NotNullIfNotNull(nameof(@object))]
        public static object? GetUnderlyingValue(object? @object)
        {
            if (@object == null)
                return null;

            if (!(@object is Enum @enum))
                throw new ArgumentException(
                    $"Type {@object.GetType()} is not an enum.",
                    nameof(@object));

            return GetUnderlyingValue(@enum);
        }

        /// <summary>
        /// Get the integer value underlying an enum value.
        /// </summary>
        /// <param name="enum">The enum value, or <see langword="null"/>.</param>
        /// <returns>The underlying integer value, or <see langword="null"/> if <paramref name="enum"/> is <see langword="null"/>.</returns>
        [return: NotNullIfNotNull(nameof(@enum))]
        public static object? GetUnderlyingValue(Enum? @enum)
        {
            if (@enum == null)
                return null;

            return TypeUtil.CastTo(@enum, @enum.GetType().GetEnumUnderlyingType());
        }

        /// <summary>
        /// Check if an enumeration value is defined.
        /// Uses <see cref="IsDefined(Enum)"/> internally.
        /// </summary>
        /// <param name="object">An enumeration value, must be an instance of <see cref="Enum"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="object"/> is a defined enumeration value, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="object"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="object"/> is not an instance of <see cref="Enum"/>.</exception>
        public static bool IsDefined(object @object)
        {
            if (@object == null)
                throw new ArgumentNullException(nameof(@object));

            if (!(@object is Enum @enum))
                throw new ArgumentException(
                    $"Type {@object.GetType()} is not an enum.",
                    nameof(@object));

            return IsDefined((Enum) @object);
        }

        /// <summary>
        /// Check if the enumeration value is defined.
        /// Uses <see cref="Enum.IsDefined(Type, object)"/> internally.
        /// </summary>
        /// <param name="enum">The enumeration value.</param>
        /// <returns><see langword="true"/> if <paramref name="enum"/> is defined, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="enum"/> is <see langword="null"/>.</exception>
        public static bool IsDefined(Enum @enum)
        {
            if (@enum == null)
                throw new ArgumentNullException(nameof(@enum));

            return Enum.IsDefined(@enum.GetType(), @enum);
        }

        /// <summary>
        /// <inheritdoc cref="IsDefined(Enum)"/>
        /// </summary>
        /// <typeparam name="T">The enumeration type.</typeparam>
        /// <param name="enum"><inheritdoc cref="IsDefined(Enum)"/></param>
        /// <returns><inheritdoc cref="IsDefined(Enum)"/></returns>
        public static bool IsDefined<T>(T @enum)
            where T : struct, Enum
        {
            return Enum.IsDefined(typeof(T), @enum);
        }

        /// <summary>
        /// Check if an enumeration value is defined or is the default value.
        /// Uses <see cref="IsDefinedOrDefault(Enum)"/> internally.
        /// </summary>
        /// <param name="object">An enumeration value, must be an instance of <see cref="Enum"/>.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="object"/> is a defined enumeration value or the default value,
        /// <see langword="false"/> otherwise.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="object"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="object"/> is not an instance of <see cref="Enum"/>.</exception>
        public static bool IsDefinedOrDefault(object @object)
        {
            if (@object == null)
                throw new ArgumentNullException(nameof(@object));

            if (!(@object is Enum @enum))
                throw new ArgumentException(
                    $"Type {@object.GetType()} is not an enum.",
                    nameof(@object));

            return IsDefinedOrDefault((Enum) @object);
        }

        /// <summary>
        /// Check if the enumeration value is defined or is the default value.
        /// Uses <see cref="Enum.IsDefined(Type, object)"/> internally.
        /// </summary>
        /// <param name="enum">The enumeration value.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="enum"/> is a defined enumeration value or the default value,
        /// <see langword="false"/> otherwise.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="enum"/> is <see langword="null"/>.</exception>
        public static bool IsDefinedOrDefault(Enum @enum)
        {
            if (@enum == null)
                throw new ArgumentNullException(nameof(@enum));

            return Enum.IsDefined(@enum.GetType(), @enum) || IsDefault(@enum);
        }

        /// <summary>
        /// <inheritdoc cref="IsDefinedOrDefault(Enum)"/>
        /// </summary>
        /// <typeparam name="T">The enumeration type.</typeparam>
        /// <param name="enum"><inheritdoc cref="IsDefinedOrDefault(Enum)"/></param>
        /// <returns><inheritdoc cref="IsDefinedOrDefault(Enum)"/></returns>
        public static bool IsDefinedOrDefault<T>(T @enum)
            where T : struct, Enum
        {
            return Enum.IsDefined(typeof(T), @enum) || object.Equals(@enum, default(T));
        }
    }
}
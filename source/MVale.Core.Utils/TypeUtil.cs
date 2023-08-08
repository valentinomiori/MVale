using System;
using System.Linq.Expressions;
using System.Reflection;

namespace MVale.Core.Utils
{
    /// <summary>
    /// A utility class related to types.
    /// Related: <seealso cref="Type"/>
    /// </summary>
    public static class TypeUtil
    {
        private static bool DefaultIsChecked => SystemInfo.DefaultIsChecked;

        internal static object CastToUsingLambda(object instance, Type type, bool isChecked)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (instance?.GetType() == type)
                return instance;

            var parameter = Expression.Parameter(typeof(object), nameof(instance));
            var body = isChecked ? Expression.ConvertChecked(parameter, type) : Expression.Convert(parameter, type);
            var lambda = Expression.Lambda(body, new[] { parameter });
            var compiled = lambda.Compile();

            try
            {
                return compiled.DynamicInvoke(instance);
            }
            catch (TargetInvocationException exception)
            {
                throw exception.InnerException;
            }
        }

        internal static object ConvertToUsingLambda(object instance, Type currentType, Type desiredType, bool isChecked)
        {
            if (currentType == null)
                throw new ArgumentNullException(nameof(desiredType));

            if (desiredType == null)
                throw new ArgumentNullException(nameof(desiredType));

            if (instance == null)
            {
                if (!IsAssignableFromNull(currentType))
                    throw new ArgumentException(
                        $"A null instance cannot be assigned to type {currentType}.",
                        nameof(instance));
            }
            else if (!currentType.IsAssignableFrom(instance.GetType()))
            {
                throw new ArgumentException(
                    $"An instance of type {instance.GetType()} is not assignable to type {currentType}.",
                    nameof(currentType));
            }

            var parameter = Expression.Parameter(currentType, nameof(instance));
            
            Expression body;
            try
            {
                body = isChecked ? Expression.ConvertChecked(parameter, desiredType) : Expression.Convert(parameter, desiredType);
            }
            catch (InvalidOperationException exception)
            {
                throw exception;
            }

            var lambda = Expression.Lambda(body, new[] { parameter });
            var compiled = lambda.Compile();

            try
            {
                return compiled.DynamicInvoke(instance);
            }
            catch (TargetInvocationException exception)
            {
                throw exception.InnerException;
            }
        }

        /// <summary>
        /// Check if <see langword="null"/> can be assigned to a variable of type <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type to perform the check for.</param>
        /// <returns>
        /// <see langword="true"/> if <see langword="null"/> can be assigned
        /// to a variable of type <paramref name="type"/>, or <see langword="false"/> otherwise.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="type"/> is <see langeord="null"/>.</exception>
        public static bool IsAssignableFromNull(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            
            if (!type.IsValueType)
                return true;

            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static object CastTo(object instance, Type type)
        {
            return CastTo(instance, type, isChecked: false);
        }

        public static object CastTo(object instance, Type type, bool? isChecked)
        {
            return CastToUsingLambda(instance, type, isChecked ?? DefaultIsChecked);
        }

        public static object ConvertTo(object instance, Type type)
        {
            return ConvertTo(instance, type, (bool?)null);
        }

        public static object ConvertTo(object instance, Type type, bool? isChecked)
        {
            return ConvertToUsingLambda(
                instance: instance,
                currentType: instance?.GetType() ?? typeof(object),
                desiredType: type,
                isChecked: isChecked ?? DefaultIsChecked);
        }

        public static object ConvertTo(object instance, Type currentType, Type desiredType)
        {
            return ConvertTo(instance, currentType, desiredType, null);
        }

        public static object ConvertTo(object instance, Type currentType, Type desiredType, bool? isChecked)
        {
            return ConvertToUsingLambda(
                instance: instance,
                currentType: currentType,
                desiredType: desiredType,
                isChecked: isChecked ?? DefaultIsChecked);
        }
    }
}
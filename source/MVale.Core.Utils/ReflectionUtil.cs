using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace MVale.Core.Utils
{
    /// <summary>
    /// A utility class to enhance the usage of reflection.
    /// </summary>
    public static class ReflectionUtil
    {
        private static ConditionalWeakTable<Type, Func<object>> DefaultValueFactories { get; }
            = new ConditionalWeakTable<Type, Func<object>>();

        internal static Func<object> CreateDefaultValueFactory(Type type)
        {
            return Expression.Lambda<Func<object>>(Expression.Default(type)).Compile();
        }

        /// <summary>
        /// Returns the type's ancestors in form of a bottom-up type sequence.
        /// </summary>
        /// <param name="type">The type for whcich the anchestors are needed, cannot be <key>null</key>.</param>
        /// <returns>
        /// A sequence of <see cref="Type"/> objects representing the inheritance chain
        /// starting from the direct base to the root, not including the parameter itself.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="type"/> is <code>null</code></exception>
        public static IEnumerable<Type> GetBaseTypes(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            while (true)
            {
                type = type.BaseType;

                if (type == null)
                    break;

                yield return type;
            }
        }

        /// <summary>
        /// Get the default value of the provided type instance.
        /// </summary>
        /// <param name="type">Type instance that represents the desired default value.</param>
        /// <returns>
        /// The default value or <see langword="null"/>
        /// if <paramref name="type"/> is a reference type.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="type"/> is <see langword="null"/>.</exception>
        public static object? GetDefaultValue(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (type.IsValueType)
            {
                return DefaultValueFactories.GetValue(type, CreateDefaultValueFactory).Invoke();
            }

            return null;
        }

        internal static MemberInfo Member<T, TMember>(Expression<Func<T, TMember>> expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            if (expression.Body is MemberExpression memberExpression)
            {
                if (memberExpression.Expression != expression.Parameters[0])
                    throw new ArgumentException(
                        $"The member expression body inner expression must be the parameter expression.",
                        nameof(expression));

                return memberExpression.Member;
            }
            /*else if (expression.Body is MethodCallExpression methodCallExpression)
            {

            }*/
            else
            {
                throw new ArgumentException(
                    $"The expression body must be a {typeof(MemberExpression)}, got: {expression}.",
                    nameof(expression));
            }
        }

        internal static MemberInfo Member<TMember>(Expression<Func<TMember>> expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            if (expression.Body is MemberExpression memberExpression)
            {
                if (memberExpression.Expression != null)
                    throw new ArgumentException(
                        $"The member expression body inner expression must be null.",
                        nameof(expression));

                return memberExpression.Member;
            }
            /*else if (expression.Body is MethodCallExpression methodCallExpression)
            {

            }*/
            else
            {
                throw new ArgumentException(
                    $"The expression body must be a {nameof(MemberExpression)}, got: {expression}.",
                    nameof(expression));
            }
        }

        internal static FieldInfo Field<T, TField>(Expression<Func<T, TField>> expression)
        {
            var memberInfo = Member(expression);

            if (memberInfo is FieldInfo fieldInfo)
            {
                return fieldInfo;
            }
            else
            {
                throw new ArgumentException(
                    $"The member {memberInfo} is not a field.",
                    nameof(expression));
            }
        }

        internal static PropertyInfo Property<T, TProperty>(Expression<Func<T, TProperty>> expression)
        {
            var memberInfo = Member(expression);

            if (memberInfo is PropertyInfo propertyInfo)
            {
                return propertyInfo;
            }
            else
            {
                throw new ArgumentException(
                    $"The member {memberInfo} is not a property.",
                    nameof(expression));
            }
        }

        internal static FieldInfo Field<TMember>(Expression<Func<TMember>> expression)
        {
            var memberInfo = Member(expression);

            if (memberInfo is FieldInfo fieldInfo)
            {
                if (!fieldInfo.IsStatic)
                    throw new ArgumentException($"The member {memberInfo} is not static.", nameof(expression));

                return fieldInfo;
            }
            else
            {
                throw new ArgumentException(
                    $"The member {memberInfo} is not a field.",
                    nameof(expression));
            }
        }

        internal static PropertyInfo Property<TProperty>(Expression<Func<TProperty>> expression)
        {
            var memberInfo = Member(expression);

            if (memberInfo is PropertyInfo propertyInfo)
            {
                if (!(propertyInfo.GetGetMethod() ?? propertyInfo.GetSetMethod()).IsStatic)
                    throw new ArgumentException(
                        $"The member {memberInfo} is not static.",
                        nameof(expression));

                return propertyInfo;
            }
            else
            {
                throw new ArgumentException(
                    $"The member {memberInfo} is not a property.",
                    nameof(expression));
            }
        }

        /// <summary>
        /// Returns all the public fields of the current <see cref="System.Type"/> including inherited.
        /// </summary>
        /// <param name="type">The current <see cref="System.Type"/></param>
        /// <returns>
        /// An sequence of <see cref="System.Reflection.FieldInfo"/> objects representing all the public fields
        /// defined for the current <see cref="System.Type"/> including inherited.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="type"/> is <code>null</code></exception>
        public static IEnumerable<FieldInfo> GetAllFields(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            foreach (var t in EnumerableUtil.Create(type).Concat(GetBaseTypes(type)))
            {
                foreach (var f in t.GetFields())
                {
                    if (f.DeclaringType == t)
                        yield return f;
                }
            }
        }

        /// <summary>
        /// Searches for the fields defined for the current
        /// <see cref="System.Type"/>, using the specified binding constraints.
        /// </summary>
        /// <param name="type">The current <see cref="System.Type"/></param>
        /// <param name="bindingAttr">
        /// A bitwise combination of the enumeration values that specify how the search is
        /// conducted.
        /// </param>
        /// <returns>
        /// An sequence of <see cref="System.Reflection.FieldInfo"/> objects representing all fields defined
        /// for the current <see cref="System.Type"/> that match the specified binding constraints.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="type"/> is <code>null</code></exception>
        public static IEnumerable<FieldInfo> GetAllFields(Type type, BindingFlags bindingAttr)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            foreach (var t in EnumerableUtil.Create(type).Concat(GetBaseTypes(type)))
            {
                foreach (var f in t.GetFields(bindingAttr))
                {
                    if (f.DeclaringType == t)
                        yield return f;
                }
            }
        }
    }
}
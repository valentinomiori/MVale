using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace MVale.Core.Utils
{
    /// <summary>
    /// Utility class related to tasks.
    /// Related: <see cref="Task"/>, <see cref="Task{T}"/>
    /// </summary>
    public static class TaskUtil
    {
        private static System.Reflection.MethodInfo InternalGetResultMethod { get; }
            = typeof(TaskUtil)
            .GetMethods(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
            .Where(m => m.IsGenericMethodDefinition)
            .Single(m => m.Name == nameof(InternalGetResult));

        private static ConditionalWeakTable<Type, Type> TaskResultTypes { get; }
            = new ConditionalWeakTable<Type, Type>();

        
        private static Type FindTaskType(Type taskType)
        {
            var genericTaskType = ReflectionUtil.GetBaseTypes(taskType)
            .Prepend(taskType)
            .Single(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Task<>));

            if (genericTaskType == null)
                throw new ArgumentException($"Type '{taskType}' is not assignable to '{typeof(Task<>)}'.", nameof(taskType));

            return genericTaskType;
        }

        private static Type FindTaskResultType(Type taskType)
        {
            return FindTaskType(taskType).GetGenericArguments().Single();
        }

        private static async Task<TResult> InternalGetResult<TValue, TResult>(Task<TValue> task)
        {
            return CastUtil.Cast<TValue, TResult>(await task);
        }

        public static Type GetTaskResultType(Type taskType)
        {
            if (TaskResultTypes == null)
                return FindTaskResultType(taskType);

            return TaskResultTypes.GetValue(taskType, FindTaskResultType);
        }

        [return: NotNullIfNotNull(nameof(task))]
        public static Task<TResult>? GetResult<TValue, TResult>(Task<TValue>? task)
        {
            if (task == null)
                return null;

            return InternalGetResult<TValue, TResult>(task);
        }
        
        [return: NotNullIfNotNull(nameof(task))]
        public static Task<T>? GetResult<T>(Task? task)
        {
            if (task == null)
                return null;

            var taskResultType = GetTaskResultType(task.GetType());
            return (Task<T>) InternalGetResultMethod.MakeGenericMethod(taskResultType, typeof(T)).Invoke(null, new object[] { task });
        }
    }
}
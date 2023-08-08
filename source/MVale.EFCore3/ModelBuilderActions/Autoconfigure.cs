using System;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MVale.EntityFramework.ModelBuilderActions
{
    /// <summary>
    /// For each entity in the model searches through its type for a definition of:
    /// <para/>
    /// - a method with a single parameter of type <see cref="ModelBuilder"/>
    /// <para/>
    /// and/or
    /// <para/>
    /// - a method with a single parameter of constructed generic type from
    /// <see cref="EntityTypeBuilder{TEntity}"/> and type parameter the entity type.
    /// <para/>
    /// Both methods must be static (any visibility) and named <see cref="MethodName"/>.
    /// Methods are invoked with an appropriate instance obtained from the current model builder.
    /// </summary>
    public sealed class Autoconfigure : IModelBuilderAction
    {
        public const string MethodName = "OnModelCreating";

        private static readonly MethodInfo ModelBuilderEntityMethod = typeof(ModelBuilder).GetMethods()
            .Where(m => !m.IsStatic && m.IsPublic)
            .Where(m => m.IsGenericMethodDefinition)
            .Where(m => m.GetGenericArguments().Length == 1)
            .Where(m => m.GetParameters().Length == 0)
            .Single(m => m.Name == nameof(ModelBuilder.Entity));

        public static Autoconfigure Instance { get; }
            = new Autoconfigure();

        private Autoconfigure()
        {
        }

        static MethodInfo GetOnModelCreatingMethod(Type type)
        {
            return type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(m => m.IsStatic)
            .Where(m => !m.IsGenericMethod && !m.IsGenericMethodDefinition)
            .Where(m => m.ReturnType == typeof(void))
            .Where(m => m.GetParameters().Length == 1 && m.GetParameters()[0].ParameterType == typeof(ModelBuilder))
            .SingleOrDefault(m => m.Name == MethodName);
        }

        static MethodInfo GetGenericOnModelCreatingMethod(Type type)
        {
            var entityTypeBuilderType = typeof(EntityTypeBuilder<>).MakeGenericType(type);

            return type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(m => m.IsStatic)
            .Where(m => !m.IsGenericMethod && !m.IsGenericMethodDefinition)
            .Where(m => m.ReturnType == typeof(void))
            .Where(m => m.GetParameters().Length == 1 && m.GetParameters()[0].ParameterType == entityTypeBuilderType)
            .SingleOrDefault(m => m.Name == MethodName);
        }

        private static void Apply(Type type, ModelBuilder modelBuilder)
        {
            GetOnModelCreatingMethod(type)?.Invoke(null, new object[] { modelBuilder });

            var genericOnModelCreatingMethod = GetGenericOnModelCreatingMethod(type);

            if(genericOnModelCreatingMethod != null)
            {
                var genericEntityTypeBuilder = ModelBuilderEntityMethod.MakeGenericMethod(type)
                .Invoke(modelBuilder, Array.Empty<object>());

                genericOnModelCreatingMethod.Invoke(null, new object[] { genericEntityTypeBuilder });
            }
        }

        public void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var type in modelBuilder.Model.GetEntityTypes().Select(et => et.ClrType))
            {
                Apply(type, modelBuilder);
            }
        }
    }
}
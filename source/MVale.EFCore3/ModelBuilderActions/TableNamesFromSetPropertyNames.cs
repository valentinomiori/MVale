using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace MVale.EntityFramework.ModelBuilderActions
{
    /// <summary>
    /// Scans a <see cref="DbContext"/> derived type in <see cref="DbContextType"/>
    /// for every public instance property that has all of the following:
    /// <para/>
    /// - declared type derived from <see cref="DbSet{TEntity}"/>.
    /// <para/>
    /// - a public get method.
    /// <para/>
    /// - a public or private set method.
    /// <para/>
    /// And sets the table name for the entity type obtained from the type parameter of <see cref="DbSet{TEntity}"/>
    /// to the value obtained from <see cref="MemberInfo.Name"/> property if no ambiguities are found.
    /// </summary>
    public class TableNamesFromSetPropertyNames : IModelBuilderAction
    {
        public Type DbContextType { get; }

        public TableNamesFromSetPropertyNames(Type dbContextType)
        {
            if (dbContextType == null)
                throw new ArgumentNullException(nameof(dbContextType));

            if (!typeof(DbContext).IsAssignableFrom(dbContextType))
                throw new ArgumentException(
                    $"Type '{dbContextType}' is not derived from '{typeof(DbContext)}'.",
                    nameof(dbContextType));

            DbContextType = dbContextType;
        }

        public static TableNamesFromSetPropertyNames Create(DbContext dbContext)
            => new TableNamesFromSetPropertyNames(dbContext?.GetType());

        public void OnModelCreating(ModelBuilder modelBuilder)
        {
            const System.Reflection.BindingFlags bindingAttr
                = System.Reflection.BindingFlags.Public
                | System.Reflection.BindingFlags.Instance;

            static Type GetDbSetEntityType(Type type)
            {
                static Type GetEntityType(Type type)
                {
                    if (type.GetGenericTypeDefinition() == typeof(DbSet<>))
                        return type.GetGenericArguments()[0];

                    return null;
                }

                while (type != typeof(object))
                {
                    var entityType = GetEntityType(type);

                    if (entityType != null)
                        return entityType;

                    type = type.BaseType;
                }

                return null;
            }

            var entityTypesToProperties = new Dictionary<Type, List<PropertyInfo>>();

            foreach (var propertyInfo in this.DbContextType.GetProperties(bindingAttr))
            {
                if (propertyInfo.GetGetMethod() == null)
                    continue;

                if (propertyInfo.GetSetMethod(nonPublic: true) == null)
                    continue;

                var entityType = GetDbSetEntityType(propertyInfo.PropertyType);

                if (entityType == null)
                    continue;

                if (!entityTypesToProperties.ContainsKey(entityType))
                {
                    entityTypesToProperties[entityType] = new List<PropertyInfo>();
                }

                entityTypesToProperties[entityType].Add(propertyInfo);
            }

            foreach (var pair in entityTypesToProperties.Where(p => p.Value.Count == 1))
            {
                modelBuilder.Entity(pair.Key).ToTable(pair.Value.Single().Name);
            }
        }
    }
}
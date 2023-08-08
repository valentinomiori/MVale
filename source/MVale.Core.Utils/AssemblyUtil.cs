using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MVale.Core.Utils
{
    public static class AssemblyUtil
    {
        public static readonly bool ReflectionOnlyLoad = true;

        static AssemblyUtil()
        {
            try
            {
                Assembly.ReflectionOnlyLoad(typeof(object).Assembly.FullName);
            }
            catch (PlatformNotSupportedException)
            {
                ReflectionOnlyLoad = false;
            }
        }

        private static Assembly GetAssemblyForReflection(AssemblyName assemblyName)
        {
            try
            {
                if (ReflectionOnlyLoad)
                {
                    try
                    {
                        return Assembly.ReflectionOnlyLoad(assemblyName.FullName);
                    }
                    catch (FileNotFoundException)
                    {
                        return Assembly.ReflectionOnlyLoad(assemblyName.Name);
                    }
                }
                else
                {
                    try
                    {
                        return Assembly.Load(assemblyName.FullName);
                    }
                    catch (FileNotFoundException)
                    {
                        return Assembly.Load(assemblyName.Name);
                    }
                }
            }
            catch (FileNotFoundException exception)
            {
                throw new InvalidOperationException($"Could not load assembly '{assemblyName}'.", exception);
            }
        }

        private static void PutAllReferencedAssemblies(
            ImmutableList<Assembly> assemblyChain,
            ISet<Assembly> assemblySet)
        {
            var assembly = assemblyChain.Last();

            foreach (var assemblyName in assembly.GetReferencedAssemblies())
            {
                Assembly referencedAssembly;
                try
                {
                    referencedAssembly = GetAssemblyForReflection(assemblyName);
                }
                catch (Exception exception)
                {
                    string path = string.Join(
                        " then loaded ",
                        assemblyChain.Select(a => $"'{a.GetName()}'"));
                    
                    throw new InvalidOperationException(
                        $"Could not load referenced assembly '{assemblyName}'. "
                        + $"Initially loaded {path} and finally loading: '{assemblyName}'.",
                        exception);
                }

                if (!assemblySet.Contains(referencedAssembly))
                {
                    assemblySet.Add(referencedAssembly);
                    PutAllReferencedAssemblies(assemblyChain.Add(referencedAssembly), assemblySet);
                }
            }
        }

        public static IReadOnlyCollection<Assembly> GetAllReferencedAssemblies()
        {
            return GetAllReferencedAssemblies(Assembly.GetCallingAssembly());
        }

        public static IReadOnlyCollection<Assembly> GetAllReferencedAssemblies(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            ISet<Assembly> assemblySet = new HashSet<Assembly>();
            PutAllReferencedAssemblies(ImmutableList.Create(assembly), assemblySet);

            return assemblySet.ToImmutableHashSet();
        }
    }
}
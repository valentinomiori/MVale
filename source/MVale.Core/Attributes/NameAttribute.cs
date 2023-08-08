using System;

namespace MVale.Core.Attributes
{
    [AttributeUsage(Targets, AllowMultiple = false, Inherited = false)]
    public sealed class NameAttribute : Attribute
    {
        public const AttributeTargets Targets
            = AttributeTargets.Assembly
            | AttributeTargets.Interface
            | AttributeTargets.Enum
            | AttributeTargets.Delegate
            | AttributeTargets.Struct
            | AttributeTargets.Class
            | AttributeTargets.Field
            | AttributeTargets.Property
            | AttributeTargets.Event
            | AttributeTargets.Constructor
            | AttributeTargets.Method
            | AttributeTargets.Parameter
            | AttributeTargets.GenericParameter;

        public string Name { get; }

        public NameAttribute(string name)
        {
            Name = name;
        }
    }
}
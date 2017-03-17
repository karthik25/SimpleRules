using System;

namespace SimpleRules.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class EntityKeyAttribute : Attribute
    {
    }
}

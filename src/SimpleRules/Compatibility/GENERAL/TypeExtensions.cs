#if !NETSTANDARD && !NET_4_5 && !NET_4_5_1 && !NET_4_5_2
using System;
using System.Reflection;

namespace SimpleRules
{
    public static class TypeExtensions
    {
        internal static TypeInfo GetTypeInfo(this Type type)
        {
            return new TypeInfo(type);
        }
    }
}

namespace System.Reflection
{
    internal class TypeInfo
    {
        private readonly Type type;

        public bool IsGenericType => type.IsGenericType;

        public bool IsInterface => type.IsInterface;

        public Assembly Assembly => type.Assembly;

        public TypeInfo(Type type)
        {
            this.type = type;
        }
    }
}
#endif

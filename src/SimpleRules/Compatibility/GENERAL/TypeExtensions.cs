#if !NETSTANDARD && !NET_4_6_1 && !NET_4_5 && !NET_4_5_1 && !NET_4_5_2
using System;
using System.Reflection;

namespace SimpleRules
{
    public static class TypeExtensions
    {
        public static bool IsInterface(this Type type)
        {
            return type.IsInterface;
        }

        public static bool IsGenericType(this Type type)
        {
            return type.IsGenericType;
        }

        public static Assembly GetAssembly(this Type type)
        {
            return type.Assembly;
        }
    }
}
#endif

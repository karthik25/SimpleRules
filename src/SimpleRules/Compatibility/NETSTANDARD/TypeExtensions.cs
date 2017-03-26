#if NETSTANDARD
using System;
using System.Reflection;
using System.Collections.Generic;

namespace SimpleRules
{
    public static class TypeExtensions
    {
        public static IEnumerable<T> GetCustomAttributes<T>(this Type type, bool inherit = false)
            where T:Attribute
        {
            return type.GetTypeInfo().GetCustomAttributes<T>(false);
        }

        public static T GetCustomAttribute<T>(this Type type, bool inherit = false)
            where T : Attribute
        {
            return type.GetTypeInfo().GetCustomAttribute<T>(false);
        }

        public static bool IsAssignableFrom(this Type type, Type type2)
        {
            return type.GetTypeInfo().IsAssignableFrom(type2);
        }

        internal static System.Reflection.TypeInfo GetTypeInfo(this Type type)
        {
            return type.GetTypeInfo();
        }
    }
}
#endif

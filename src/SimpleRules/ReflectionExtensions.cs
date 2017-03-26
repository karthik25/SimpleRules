using System;
using System.Linq;
using SimpleRules.Contracts;
using System.Collections.Generic;

namespace SimpleRules
{
    public static class ReflectionExtensions
    {
        public static IEnumerable<Type> FindHandlerTypesInAssemblies(this Type[] assemblyMarkerTypes)
        {
            var assemblies = assemblyMarkerTypes.Select(a => a.GetTypeInfo().Assembly);
            var handlerTypes = assemblies
                                    .SelectMany(a => a.GetTypes())
                                    .Where(a => typeof(IHandler).IsAssignableFrom(a) && 
                                                !a.GetTypeInfo().IsInterface);
            return handlerTypes;
        }

        public static IHandler CreateInstance(this Type type)
        {
            var instance = Activator.CreateInstance(type);
            return instance as IHandler;
        }
    }
}

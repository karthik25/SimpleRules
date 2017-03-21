using System;
using System.Linq;
using SimpleRules.Contracts;

namespace SimpleRules
{
    public static class ReflectionExtensions
    {
        public static IHandler[] FindHandlersInAssemblies(this Type[] assemblyMarkerTypes)
        {
            var assemblies = assemblyMarkerTypes.Select(a => a.Assembly);
            var handlerTypes = assemblies.SelectMany(a => a.GetTypes()).Where(a => typeof(IHandler).IsAssignableFrom(a));
            var instances = handlerTypes.Select(h => Activator.CreateInstance(h)).Cast<IHandler>().ToArray();
            return instances;
        }
    }
}

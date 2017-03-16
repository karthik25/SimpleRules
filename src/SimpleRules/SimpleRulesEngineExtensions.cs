using System;
using SimpleRules.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SimpleRules.Contracts;
using SimpleRules.Exceptions;

namespace SimpleRules
{
    public static class SimpleRulesEngineExtensions
    {
        public static IEnumerable<ValidationResult> Validate<TConcrete>(this SimpleRulesEngine engine, List<TConcrete> src)
            where TConcrete : class
        {
            return engine.Validate(src);
        }

        public static bool HasRuleAttributes(this Type type)
        {
            return type.GetProperties(publicPropFlags)
                       .SelectMany(p => p.GetCustomAttributes<BaseRuleAttribute>(false))
                       .Any();
        }

        public static bool HasRuleMetadataAttribute(this Type type)
        {
            return type.GetCustomAttributes<RuleMetadataAttribute>(false)
                       .Any();
        }

        public static Type FindRuleMetadataType(this Type type)
        {
            var ruleMetaAttr = type.GetCustomAttribute<RuleMetadataAttribute>();
            return ruleMetaAttr.MetaFor;
        }

        // todo: should return a Tuple<BaseRuleAttribute, PropertyInfo>
        public static List<BaseRuleAttribute> GetRules(this Type srcType)
        {
            return srcType.GetProperties(publicPropFlags)
                          .SelectMany(p => p.GetCustomAttributes<BaseRuleAttribute>(false))
                          .ToList();
        }

        // todo: should return a Tuple<BaseRuleAttribute, PropertyInfo>
        public static List<BaseRuleAttribute> GetRules(this Type srcType, Type metadataType)
        {
            var srcProperties = srcType.GetProperties(publicPropFlags);
            var metaProperties = metadataType.GetProperties(publicPropFlags);
            var matchedMetaProperties = srcProperties
                                           .Select(p => p.GetMatchingPropertyInfo(metaProperties))
                                           .Where(r => r != null);
            var ruleAttributes = 
                        matchedMetaProperties
                            .SelectMany(p => p.GetCustomAttributes<BaseRuleAttribute>())
                            .ToList();
            return ruleAttributes;
        }

        private static PropertyInfo GetMatchingPropertyInfo(this PropertyInfo srcProperty, IEnumerable<PropertyInfo> metaProperties)
        {
            return metaProperties.SingleOrDefault(s => 
                        s.Name == srcProperty.Name && 
                        (s.PropertyType == srcProperty.PropertyType || 
                        s.PropertyType == typeof(object)));
        }
        
        public static bool All(this List<ValidationResult> results)
        {
            return results.All(r => r.IsError);
        }

        public static bool Any(this List<ValidationResult> results)
        {
            return results.Any(r => r.IsError);
        }

        public static Type GetMetadataType(this Dictionary<Type, Type> typeMetaDictionary, Type srcType)
        {
            if (typeMetaDictionary.ContainsKey(srcType))
                return typeMetaDictionary[srcType];

            if (srcType.HasRuleMetadataAttribute())
                return srcType.FindRuleMetadataType();

            if (srcType.HasRuleAttributes())
                return srcType;

            return null;
        }

        public static IHandler FindHandler(this Dictionary<Type, IHandler> attrHandlerMapping, BaseRuleAttribute attribute)
        {
            var handler = attrHandlerMapping.Values.SingleOrDefault(h => h.Handles(attribute));
            if (handler == null)
            {
                throw new HandlerNotFoundException(attribute.GetType().Name, attribute.GetType().FullName);
            }
            return handler;
        }

        private static BindingFlags publicPropFlags = BindingFlags.Instance | BindingFlags.Public;
    }
}

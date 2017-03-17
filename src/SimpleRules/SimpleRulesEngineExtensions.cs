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
        
        public static IEnumerable<Tuple<BaseRuleAttribute, PropertyInfo>> GetRules(this Type srcType)
        {
            var propList = srcType.GetProperties(publicPropFlags)
                                  .Where(p => p.GetCustomAttributes<BaseRuleAttribute>().Any());
            foreach (var prop in propList)
            {
                var attrs = prop.GetCustomAttributes<BaseRuleAttribute>();
                foreach (var attr in attrs)
                {
                    yield return new Tuple<BaseRuleAttribute, PropertyInfo>(attr, prop);
                }
            }
        }

        public static IEnumerable<Tuple<BaseRuleAttribute, PropertyInfo>> GetRules(this Type srcType, Type metadataType)
        {
            var srcProperties = srcType.GetProperties(publicPropFlags);
            var metaProperties = metadataType.GetProperties(publicPropFlags);
            var matchedMetaProperties = srcProperties
                                           .Select(p => p.GetMatchingPropertyInfo(metaProperties))
                                           .Where(r => r != null);
            foreach (var prop in matchedMetaProperties)
            {
                var attrs = prop.GetCustomAttributes<BaseRuleAttribute>();
                foreach (var attr in attrs)
                {
                    yield return new Tuple<BaseRuleAttribute, PropertyInfo>(attr, prop);
                }
            }
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

        public static Tuple<Type, Type> GetMetadataType(this Dictionary<Type, Type> typeMetaDictionary, Type srcType)
        {
            if (typeMetaDictionary.ContainsKey(srcType))
                return new Tuple<Type, Type>(srcType, typeMetaDictionary[srcType]);

            if (srcType.HasRuleMetadataAttribute())
                return new Tuple<Type, Type>(srcType, srcType.FindRuleMetadataType());

            if (srcType.HasRuleAttributes())
                return new Tuple<Type, Type>(srcType, srcType);

            return null;
        }

        public static PropertyInfo FindEntityKeyPropertyInfo(this Tuple<Type, Type> ruleMetaMap)
        {
            if (ruleMetaMap.Item1 == ruleMetaMap.Item2)
            {
                var propertyWithEntityKeyAttr = ruleMetaMap.Item1
                                                           .GetProperties(publicPropFlags)
                                                           .FirstOrDefault(p => p.GetCustomAttributes<EntityKeyAttribute>().Any());
                if (propertyWithEntityKeyAttr != null)
                    return propertyWithEntityKeyAttr;
                else
                    return null;
            }
            var propertyWithEntityKeyAttrMeta = ruleMetaMap.Item2
                                                       .GetProperties(publicPropFlags)
                                                       .FirstOrDefault(p => p.GetCustomAttributes<EntityKeyAttribute>().Any());
            if (propertyWithEntityKeyAttrMeta != null)
            {
                var matchedProperty = ruleMetaMap.Item1.GetProperties().FirstOrDefault(p => p.Name == propertyWithEntityKeyAttrMeta.Name && p.PropertyType == propertyWithEntityKeyAttrMeta.PropertyType);
                if (matchedProperty != null)
                {
                    return matchedProperty;
                }
            }
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

        public static IEnumerable<Tuple<BaseRuleAttribute, PropertyInfo>> GetRules(this Tuple<Type, Type> metaMapping)
        {
            if (metaMapping.Item1 == metaMapping.Item2)
                return metaMapping.Item1.GetRules();
            return metaMapping.Item1.GetRules(metaMapping.Item2);
        }

        private static BindingFlags publicPropFlags = BindingFlags.Instance | BindingFlags.Public;
    }
}

using System;
using System.Linq;
using System.Reflection;
using SimpleRules.Attributes;
using System.Collections.Generic;

namespace SimpleRules
{
    public static class SimpleRulesEngineExtensions
    {
        public static IEnumerable<ValidationResult> Validate<TConcrete>(this SimpleRulesEngine engine, List<TConcrete> src)
            where TConcrete : class
        {
            return engine.Validate(src);
        }

        public static ValidationResult Validate<TConcrete>(this SimpleRulesEngine engine, TConcrete src)
            where TConcrete : class
        {
            return engine.Validate(new List<TConcrete> { src }).First();
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
            var attrList = srcType.GetProperties(publicPropFlags)
                                  .SelectMany(c => c.GetCustomAttributes<BaseRuleAttribute>(), (prop, attr) => new { prop, attr })
                                  .Select(p => new Tuple<BaseRuleAttribute, PropertyInfo>(p.attr, p.prop));
            return attrList;
        }

        public static IEnumerable<Tuple<BaseRuleAttribute, PropertyInfo>> GetRules(this Type srcType, Type metadataType)
        {
            var srcProperties = srcType.GetProperties(publicPropFlags);
            var metaProperties = metadataType.GetProperties(publicPropFlags);
            var matchedMetaProperties = srcProperties
                                           .Select(p => p.GetMatchingPropertyInfo(metaProperties))
                                           .Where(r => r != null);            
            var attrList = matchedMetaProperties
                            .SelectMany(c => c.GetCustomAttributes<BaseRuleAttribute>(), (prop, attr) => new { prop, attr })
                            .Select(p => new Tuple<BaseRuleAttribute, PropertyInfo>(p.attr, p.prop));
            return attrList;
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

        public static IEnumerable<Tuple<BaseRuleAttribute, PropertyInfo>> GetRules(this Tuple<Type, Type> metaMapping)
        {
            if (metaMapping.Item1 == metaMapping.Item2)
                return metaMapping.Item1.GetRules();
            return metaMapping.Item1.GetRules(metaMapping.Item2);
        }

        public static object GetEntityKey<TConcrete>(this Dictionary<Type, PropertyInfo> entityKeyCache, TConcrete item)
        {
            if (entityKeyCache.ContainsKey(typeof(TConcrete)))
            {
                var propertyInfo = entityKeyCache[typeof(TConcrete)];
                var keyValue = propertyInfo.GetValue(item);
                return keyValue;
            }
            return null;
        }

        private static BindingFlags publicPropFlags = BindingFlags.Instance | BindingFlags.Public;
    }
}

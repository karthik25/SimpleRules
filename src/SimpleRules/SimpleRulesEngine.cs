using System;
using SimpleRules.Contracts;
using SimpleRules.Attributes;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Linq;
using SimpleRules.Generic;
using SimpleRules.Handlers;
using System.Reflection;

namespace SimpleRules
{
    public class SimpleRulesEngine
    {
        private readonly Dictionary<Type, EvaluatedRule[]> typeRulesCache = new Dictionary<Type, EvaluatedRule[]>();
        private readonly Dictionary<Type, PropertyInfo> entityKeyCache = new Dictionary<Type, PropertyInfo>();
        private readonly Dictionary<Type, Type> typeMetaCache = new Dictionary<Type, Type>();
        private readonly Dictionary<Type, IHandler> attrHandlerMapping = new Dictionary<Type, IHandler>();

        public SimpleRulesEngine() { }
        
        public IEnumerable<ValidationResult> Validate<TConcrete>(List<TConcrete> src)
            where TConcrete : class
        {
            if (!attrHandlerMapping.Any())
                AddDefaultHandlers();

            var rules = GetRules<TConcrete>();
            foreach (var item in src)
            {
                var validationResult = new ValidationResult();
                if (entityKeyCache.ContainsKey(typeof(TConcrete)))
                {
                    var propertyInfo = entityKeyCache[typeof (TConcrete)];
                    var keyValue = propertyInfo.GetValue(item);
                    validationResult.Key = keyValue;
                }
                foreach (var rule in rules)
                {
                    var message = rule.MessageFormat;
                    var compiledExpression = GetCompiledRule<TConcrete>(rule.Expression);
                    if (!compiledExpression(item))
                    {
                        validationResult.Errors.Add(message);
                    }
                }
                yield return validationResult;
            }
        }

        private void AddDefaultHandlers()
        {
            attrHandlerMapping.Add(typeof(RelationalOperatorAttribute), new SimpleRuleHandler());
        }

        public SimpleRulesEngine RegisterMetadata<TConcrete, TMeta>()
            where TConcrete : class
            where TMeta : class
        {
            typeMetaCache.Add(typeof(TConcrete), typeof(TMeta));
            return this;
        }

        public SimpleRulesEngine RegisterCustomRule<RAttr, Rhandler>()
            where RAttr : BaseRuleAttribute
            where Rhandler : IHandler, new()
        {
            throw new NotImplementedException();
        }

        private EvaluatedRule[] GetRules<TConcrete>()
        {
            var type = typeof (TConcrete);

            if (typeRulesCache.ContainsKey(type))
                return typeRulesCache[type];

            var metaDataType = typeMetaCache.GetMetadataType(type);
            if (metaDataType == null)
                throw new Exception(string.Format("Unable to identify rule metadata for the entity: {0}", type.FullName));

            var entityKeyProp = metaDataType.FindEntityKeyPropertyInfo();
            if (entityKeyProp != null)
            {
                entityKeyCache.Add(type, entityKeyProp);
            }

            var rulePropertyMap = metaDataType.GetRules()
                                              .Select(m => attrHandlerMapping.ProcessRule<TConcrete>(m.Item1, m.Item2))
                                              .ToArray();
            typeRulesCache[type] = rulePropertyMap;
            return rulePropertyMap;
        }

        private Func<T, bool> GetCompiledRule<T>(LambdaExpression expression)
            where T:class
        {
            return GetFunc<T>(expression.Compile());
        }

        private static Func<T, bool> GetFunc<T>(Delegate funcDelegate)
        {
            return (Func<T, bool>)funcDelegate;
        }
    }
}

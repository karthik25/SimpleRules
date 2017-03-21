using System;
using SimpleRules.Contracts;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Linq;
using SimpleRules.Handlers;
using System.Reflection;

namespace SimpleRules
{
    public class SimpleRulesEngine
    {
        private readonly Dictionary<Type, EvaluatedRule[]> typeRulesCache = new Dictionary<Type, EvaluatedRule[]>();
        private readonly Dictionary<Type, PropertyInfo> entityKeyCache = new Dictionary<Type, PropertyInfo>();
        private readonly Dictionary<Type, Type> typeMetaCache = new Dictionary<Type, Type>();
        private readonly List<IHandler> handlerList = new List<IHandler>();

        public SimpleRulesEngine()
        {
            AddDefaultHandlers();
        }
        
        public IEnumerable<ValidationResult> Validate<TConcrete>(List<TConcrete> src)
            where TConcrete : class
        {
            var rules = GetRules<TConcrete>();
            foreach (var item in src)
            {
                var validationResult = new ValidationResult();
                validationResult.Key = entityKeyCache.GetEntityKey(item);
                foreach (var rule in rules)
                {
                    var message = rule.MessageFormat;
                    var compiledExpression = GetCompiledRule<TConcrete>(rule.Expression);
                    if (!compiledExpression(item))
                    {
                        validationResult.Add(message, rule.RuleType);
                    }
                }
                yield return validationResult;
            }
        }

        private void AddDefaultHandlers()
        {
            handlerList.Add(new SimpleRuleHandler());
            handlerList.Add(new RegexRuleHandler());
        }

        public SimpleRulesEngine RegisterMetadata<TConcrete, TMeta>()
            where TConcrete : class
            where TMeta : class
        {
            typeMetaCache.Add(typeof(TConcrete), typeof(TMeta));
            return this;
        }

        public SimpleRulesEngine RegisterCustomRule<Rhandler>()
            where Rhandler : IHandler, new()
        {
            handlerList.Add(new Rhandler());
            return this;
        }

        public SimpleRulesEngine DiscoverHandlers(params Type[] assemblyMarkers)
        {
            var discoveredHandlers = assemblyMarkers.FindHandlersInAssemblies();
            handlerList.AddRange(discoveredHandlers);
            return this;
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
                                              .Select(m => handlerList.ProcessRule<TConcrete>(m.Item1, m.Item2))
                                              .ToArray();
            typeRulesCache[type] = rulePropertyMap;
            return rulePropertyMap;
        }

        private static Func<TConcrete, bool> GetCompiledRule<TConcrete>(LambdaExpression expression)
            where TConcrete : class
        {
            return GetFunc<TConcrete>(expression.Compile());
        }

        private static Func<TConcrete, bool> GetFunc<TConcrete>(Delegate funcDelegate)
        {
            return (Func<TConcrete, bool>)funcDelegate;
        }
    }
}

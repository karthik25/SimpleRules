using System;
using SimpleRules.Contracts;
using SimpleRules.Attributes;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace SimpleRules
{
    public class SimpleRulesEngine
    {
        private readonly Dictionary<Type, EvaluatedRule[]> typeRulesCache = new Dictionary<Type, EvaluatedRule[]>();
        private readonly Dictionary<Type, Type> typeMetaCache = new Dictionary<Type, Type>();
        private readonly Dictionary<Type, IHandler> attrHandlerMapping = new Dictionary<Type, IHandler>();

        public SimpleRulesEngine() { }
        
        public IEnumerable<ValidationResult> Validate<TConcrete>(List<TConcrete> src)
            where TConcrete : class
        {
            throw new NotImplementedException();
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

        private EvaluatedRule[] GetRules(Type type)
        {
            if (typeMetaCache.ContainsKey(type))
                return typeRulesCache[type];

            var metaDataType = typeMetaCache.GetMetadataType(type);
            if (metaDataType == null)
                throw new Exception(string.Format("Unable to identify rule metadata for the entity: {0}", type.FullName));

            return null;
        }
    }
}

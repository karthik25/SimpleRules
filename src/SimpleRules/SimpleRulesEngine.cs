using System;
using SimpleRules.Contracts;
using SimpleRules.Attributes;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace SimpleRules
{
    public class SimpleRulesEngine
    {
        private readonly Dictionary<Type, LambdaExpression[]> typeRulesCache = new Dictionary<Type, LambdaExpression[]>();
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
            throw new NotImplementedException();
        }

        public SimpleRulesEngine RegisterCustomRule<RAttr, Rhandler>()
            where RAttr : BaseRuleAttribute
            where Rhandler : IHandler, new()
        {
            throw new NotImplementedException();
        }
    }
}

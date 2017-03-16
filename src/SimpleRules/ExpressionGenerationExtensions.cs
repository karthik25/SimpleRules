using System;
using SimpleRules.Attributes;
using SimpleRules.Contracts;
using System.Collections.Generic;
using System.Reflection;

namespace SimpleRules
{
    public static class ExpressionGenerationExtensions
    {
        public static EvaluatedRule ProcessRule(this Dictionary<Type, IHandler> handlerDictionary, 
                                                        BaseRuleAttribute attribute, PropertyInfo targetProp)
        {
            throw new NotImplementedException();
        }
    }
}

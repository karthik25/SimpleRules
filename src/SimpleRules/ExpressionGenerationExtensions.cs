using System;
using SimpleRules.Attributes;
using SimpleRules.Contracts;
using System.Collections.Generic;
using System.Reflection;

namespace SimpleRules
{
    public static class ExpressionGenerationExtensions
    {
        public static EvaluatedRule ProcessRule<TConcrete>(this Dictionary<Type, IHandler> handlerDictionary, 
                                                        BaseRuleAttribute attribute, PropertyInfo targetProp)
        {
            var handler = handlerDictionary.FindHandler(attribute);
            return handler.GenerateEvaluatedRule<TConcrete>(attribute, targetProp);
        }
    }
}

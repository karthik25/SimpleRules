using SimpleRules.Attributes;
using SimpleRules.Contracts;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SimpleRules
{
    public static class ExpressionGenerationExtensions
    {
        public static LambdaExpression ProcessRule(this Dictionary<Type, IHandler> handlerDictionary, 
                                                        BaseRuleAttribute attribute)
        {
            throw new NotImplementedException();
        }
    }
}

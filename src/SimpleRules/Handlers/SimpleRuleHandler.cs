using SimpleRules.Attributes;
using SimpleRules.Contracts;
using SimpleRules.Generic;
using System;
using System.Linq.Expressions;

namespace SimpleRules.Handlers
{
    public class SimpleRuleHandler : IHandler
    {
        public LambdaExpression GenerateExpression(BaseRuleAttribute attribute)
        {
            throw new NotImplementedException();
        }

        public bool Handles(BaseRuleAttribute attribute)
        {
            return typeof(RelationalOperatorAttribute).IsAssignableFrom(attribute.GetType());
        }
    }
}

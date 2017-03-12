using SimpleRules.Attributes;
using SimpleRules.Contracts;
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
            throw new NotImplementedException();
        }
    }
}

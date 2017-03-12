using System;
using System.Linq.Expressions;
using SimpleRules.Attributes;
using SimpleRules.Contracts;

namespace SimpleRules.Handlers
{
    public class RegexRuleHandler : IHandler
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

using System;
using SimpleRules.Attributes;
using SimpleRules.Contracts;
using System.Reflection;

namespace SimpleRules.Handlers
{
    public class RegexRuleHandler : IHandler
    {
        public EvaluatedRule GenerateEvaluatedRule<T>(BaseRuleAttribute attribute, PropertyInfo targetProp)
        {
            throw new NotImplementedException();
        }

        public bool Handles(BaseRuleAttribute attribute)
        {
            throw new NotImplementedException();
        }
    }
}

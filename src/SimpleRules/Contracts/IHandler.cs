using SimpleRules.Attributes;
using System.Reflection;

namespace SimpleRules.Contracts
{
    public interface IHandler
    {
        bool Handles(BaseRuleAttribute attribute);
        EvaluatedRule GenerateEvaluatedRule(BaseRuleAttribute attribute, PropertyInfo targetProp);
    }
}

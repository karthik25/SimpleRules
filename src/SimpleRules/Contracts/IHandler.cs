using SimpleRules.Attributes;
using System.Reflection;

namespace SimpleRules.Contracts
{
    public interface IHandler
    {
        bool Handles(BaseRuleAttribute attribute);
        EvaluatedRule GenerateEvaluatedRule<T>(BaseRuleAttribute attribute, PropertyInfo targetProp);
    }
}

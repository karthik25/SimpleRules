using SimpleRules.Attributes;
using System.Linq.Expressions;

namespace SimpleRules.Contracts
{
    public interface IHandler
    {
        bool Handles(BaseRuleAttribute attribute);
        LambdaExpression GenerateExpression(BaseRuleAttribute attribute);
    }
}

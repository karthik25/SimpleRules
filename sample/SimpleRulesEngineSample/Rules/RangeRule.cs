using SimpleRules;
using SimpleRules.Attributes;
using SimpleRules.Contracts;
using System.Linq.Expressions;
using System.Reflection;

namespace SimpleRulesEngineSample.Rules
{
    public class RangeRuleAttribute : BaseRuleAttribute
    {
        public RangeRuleAttribute(int minValue, int maxValue)
        {
            MinValue = minValue;
            MaxValue = maxValue;
        }

        public int MinValue { get; set; }
        public int MaxValue { get; set; }
    }

    public class RangeRuleHandler : IHandler
    {
        public EvaluatedRule GenerateEvaluatedRule<TConcrete>(BaseRuleAttribute attribute, PropertyInfo targetProp)
        {
            var rangeAttr = attribute as RangeRuleAttribute;
            var input = Expression.Parameter(typeof(TConcrete), "a");
            var propName = targetProp.Name;
            var comparison = Expression.And(
                    Expression.GreaterThan(Expression.PropertyOrField(input, propName), Expression.Constant(rangeAttr.MinValue)),
                    Expression.LessThan(Expression.PropertyOrField(input, propName), Expression.Constant(rangeAttr.MaxValue))
                );
            var lambda = Expression.Lambda(comparison, input);
            return new EvaluatedRule
            {
                MessageFormat = string.Format("{0} should be between {1} and {2}", propName.AddSpaces(), rangeAttr.MinValue, rangeAttr.MaxValue),
                RuleType = RuleType.Error,
                Expression = lambda
            };
        }

        public bool Handles(BaseRuleAttribute attribute)
        {
            return typeof(RangeRuleAttribute).IsAssignableFrom(attribute.GetType());
        }
    }
}

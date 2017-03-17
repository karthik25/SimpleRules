using SimpleRules.Attributes;
using SimpleRules.Contracts;
using SimpleRules.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace SimpleRules.Handlers
{
    public class SimpleRuleHandler : IHandler
    {
        public EvaluatedRule GenerateEvaluatedRule<TConcrete>(BaseRuleAttribute attribute, PropertyInfo targetProp)
        {
            var relationalAttr = attribute as RelationalOperatorAttribute;
            var input = Expression.Parameter(typeof(TConcrete), "i");
            var leftExpr = Expression.PropertyOrField(input, targetProp.Name);
            var rightExpr = Expression.PropertyOrField(input, relationalAttr.OtherPropertyName);
            var binaryExpr = Expression.MakeBinary(relationalAttr.SupportedType, leftExpr, rightExpr);
            var lambdaExpr = Expression.Lambda(binaryExpr, input);
            var message = string.Format("{0} should be {1} than the {2}", targetProp.Name.AddSpaces(), "greater", relationalAttr.OtherPropertyName.AddSpaces());
            return new EvaluatedRule
            {
                MessageFormat = message,
                Expression = lambdaExpr
            };
        }

        public bool Handles(BaseRuleAttribute attribute)
        {
            return typeof(RelationalOperatorAttribute).IsAssignableFrom(attribute.GetType());
        }
    }
}

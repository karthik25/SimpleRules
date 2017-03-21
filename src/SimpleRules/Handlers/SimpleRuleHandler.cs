using SimpleRules.Attributes;
using SimpleRules.Contracts;
using SimpleRules.Exceptions;
using SimpleRules.Generic;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace SimpleRules.Handlers
{
    public class SimpleRuleHandler : IHandler
    {
        public EvaluatedRule GenerateEvaluatedRule<TConcrete>(BaseRuleAttribute attribute, PropertyInfo targetProp)
        {
            var expressions = new List<Expression>();
            var relationalAttr = attribute as RelationalOperatorAttribute;
            var input = Expression.Parameter(typeof(TConcrete), "i");
            var leftExpr = Expression.PropertyOrField(input, targetProp.Name);
            var isNullable = targetProp.IsNullable();
            if (relationalAttr.CanBeNull)
            {
                if (isNullable)
                {
                    var nullConstant = leftExpr.CreateBinaryExpression(targetProp);
                    expressions.Add(nullConstant);
                }
                else
                {
                    throw new NotNullablePropertyException(targetProp.Name);
                }
            }
            if (relationalAttr.ConstantValue != null)
            {
                var constantValue = Expression.Constant(relationalAttr.ConstantValue);
                BinaryExpression finalExpr = leftExpr.CreateBinaryExpression(constantValue, relationalAttr.SupportedType, targetProp);                
                expressions.Add(finalExpr);
            }
            if (!string.IsNullOrEmpty(relationalAttr.OtherPropertyName))
            {
                var rightExpr = Expression.PropertyOrField(input, relationalAttr.OtherPropertyName);
                var propExpr = leftExpr.CreateBinaryExpression(rightExpr, relationalAttr.SupportedType, targetProp);
                expressions.Add(propExpr);
            }
            var orExpr = expressions.CreateBinaryExpression();
            var lambdaExpr = Expression.Lambda(orExpr, input);
            var message = string.Format("{0} should be {1} the {2}", targetProp.Name.AddSpaces(), relationalAttr.SupportedType.ToString().AddSpaces(), relationalAttr.OtherPropertyName.AddSpaces());
            return new EvaluatedRule
            {
                MessageFormat = message,
                Expression = lambdaExpr,
                RuleType = relationalAttr.RuleType
            };
        }

        public bool Handles(BaseRuleAttribute attribute)
        {
            return typeof(RelationalOperatorAttribute).IsAssignableFrom(attribute.GetType());
        }
    }
}

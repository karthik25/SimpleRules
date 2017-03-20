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
            UnaryExpression nullableLeftExpr = null;
            if (isNullable)
            {
                nullableLeftExpr = Expression.Convert(leftExpr, targetProp.PropertyType);
            }
            if (relationalAttr.CanBeNull)
            {
                if (isNullable)
                {
                    var nullConstant = Expression.Equal(
                            nullableLeftExpr,
                            Expression.Constant(null)
                        );
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
                BinaryExpression finalExpr = null;
                if (!isNullable)
                {
                    finalExpr = Expression.MakeBinary(
                                        relationalAttr.SupportedType,
                                        leftExpr,
                                        constantValue
                                      );
                }
                else
                {
                    finalExpr = Expression.MakeBinary(
                                        relationalAttr.SupportedType,
                                        nullableLeftExpr,
                                        Expression.Convert(constantValue, targetProp.PropertyType)
                                      );
                }
                expressions.Add(finalExpr);
            }

            if (!string.IsNullOrEmpty(relationalAttr.OtherPropertyName))
            {
                var rightExpr = Expression.PropertyOrField(input, relationalAttr.OtherPropertyName);

                BinaryExpression propExpr = null;
                if (!isNullable)
                {
                    propExpr = Expression.MakeBinary(
                                    relationalAttr.SupportedType,
                                    leftExpr,
                                    rightExpr
                               );
                }
                else
                {
                    propExpr = Expression.MakeBinary(
                                  relationalAttr.SupportedType,
                                  nullableLeftExpr,
                                  Expression.Convert(rightExpr, targetProp.PropertyType)
                               );
                }
                expressions.Add(propExpr);
            }

            var orExpr = expressions.CreateBinaryExpressionFromList();
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

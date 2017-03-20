using System;
using SimpleRules.Attributes;
using SimpleRules.Contracts;
using System.Collections.Generic;
using System.Reflection;
using System.Linq.Expressions;
using System.Linq;

namespace SimpleRules
{
    public static class ExpressionGenerationExtensions
    {
        public static EvaluatedRule ProcessRule<TConcrete>(this Dictionary<Type, IHandler> handlerDictionary, 
                                                        BaseRuleAttribute attribute, PropertyInfo targetProp)
        {
            var handler = handlerDictionary.FindHandler(attribute);
            return handler.GenerateEvaluatedRule<TConcrete>(attribute, targetProp);
        }

        public static BinaryExpression CreateBinaryExpressionFromList(this List<Expression> expressions)
        {
            var aggregatedExpr = expressions.Aggregate((e1, e2) =>
            {
                return Expression.Or(e1, e2);
            });
            return (BinaryExpression) aggregatedExpr;
        }

        public static BinaryExpression CreateBinaryExpression(this MemberExpression leftExpr, 
                                                                   MemberExpression rightExpr, 
                                                                   ExpressionType exprType, 
                                                                   PropertyInfo propertyInfo)
        {
            var isNullable = propertyInfo.IsNullable();
            var propType = propertyInfo.PropertyType;
            if (isNullable)
            {
                return Expression.MakeBinary(
                            exprType, 
                            Expression.Convert(leftExpr, propType), 
                            Expression.Convert(rightExpr, propType));
            }
            else
            {
                return Expression.MakeBinary(
                                exprType,
                                leftExpr,
                                rightExpr
                            );
            }
        }

        public static bool IsNullable(this PropertyInfo propertyInfo)
        {
            return propertyInfo.PropertyType.IsGenericType && 
                   propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
    }
}

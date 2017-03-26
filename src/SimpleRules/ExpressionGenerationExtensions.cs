using System;
using SimpleRules.Attributes;
using SimpleRules.Contracts;
using System.Collections.Generic;
using System.Reflection;
using System.Linq.Expressions;
using System.Linq;
using SimpleRules.Exceptions;

namespace SimpleRules
{
    public static class ExpressionGenerationExtensions
    {
        public static EvaluatedRule ProcessRule<TConcrete>(this Dictionary<Type, IHandler> handlerMapping, 
                                                        BaseRuleAttribute attribute, PropertyInfo targetProp)
        {
            var handler = handlerMapping.FindHandler(attribute);
            return handler.GenerateEvaluatedRule<TConcrete>(attribute, targetProp);
        }

        public static IHandler FindHandler(this Dictionary<Type, IHandler> handlerMapping, BaseRuleAttribute attribute)
        {
            var handler = handlerMapping.Values.SingleOrDefault(h => h.Handles(attribute));
            if (handler == null)
            {
                var attrType = attribute.GetType();
                throw new HandlerNotFoundException(attrType.Name, attrType.FullName);
            }
            return handler;
        }

        public static BinaryExpression CreateBinaryExpression(this List<Expression> expressions)
        {
            var aggregatedExpr = expressions.Aggregate((e1, e2) =>
            {
                return Expression.Or(e1, e2);
            });
            return (BinaryExpression) aggregatedExpr;
        }

        public static BinaryExpression CreateBinaryExpression(this Expression expr, PropertyInfo propertyInfo)
        {
            var nullableExpr = Expression.Convert(expr, propertyInfo.PropertyType);
            var nullConst = Expression.Constant(null);
            return Expression.MakeBinary(ExpressionType.Equal, nullableExpr, nullConst);
        }

        public static BinaryExpression CreateBinaryExpression(this Expression leftExpr, 
                                                                   Expression rightExpr, 
                                                                   ExpressionType exprType, 
                                                                   PropertyInfo propertyInfo)
        {
            var isNullable = propertyInfo.IsNullable();
            var propType = propertyInfo.PropertyType;
            var leftExprFinal = isNullable ? Expression.Convert(leftExpr, propertyInfo.PropertyType) : leftExpr;
            var rightExprFinal = isNullable ? Expression.Convert(rightExpr, propertyInfo.PropertyType) : rightExpr;
            return Expression.MakeBinary(
                        exprType,
                        leftExprFinal,
                        rightExprFinal
                   );
        }

        public static bool IsNullable(this PropertyInfo propertyInfo)
        {
            return propertyInfo.PropertyType.IsGenericType() && 
                   propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
    }
}

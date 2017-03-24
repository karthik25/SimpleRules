using SimpleRules.Attributes;
using SimpleRules.Contracts;
using System.Reflection;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Linq;

namespace SimpleRules.Handlers
{
    public class RegexRuleHandler : IHandler
    {
        public EvaluatedRule GenerateEvaluatedRule<TConcrete>(BaseRuleAttribute attribute, PropertyInfo targetProp)
        {
            var regexAttr = attribute as RegexRuleAttribute;
            var input = Expression.Parameter(typeof(TConcrete), "i");
            var ctor = typeof(Regex).GetConstructors().SingleOrDefault(c => c.GetParameters().Count() == 1);
            var method = typeof(Regex).GetMethods()
                                      .SingleOrDefault(m => m.Name == "IsMatch" && 
                                                            !m.IsStatic && 
                                                            m.GetParameters().Count() == 1);
            var leftExpr = Expression.PropertyOrField(input, targetProp.Name);
            var block = Expression.Block(
                            Expression.Call(
                                Expression.New(
                                    ctor, 
                                    Expression.Constant(regexAttr.RegularExpression)), method, leftExpr)
                        );
            var expression = Expression.Lambda(block, input);
            return new EvaluatedRule
            {
                RuleType = RuleType.Error,
                MessageFormat = string.Format("{0} does not match the expected format", targetProp.Name.AddSpaces(), regexAttr.RegularExpression),
                Expression = expression
            };
        }

        public bool Handles(BaseRuleAttribute attribute)
        {
            return typeof(RegexRuleAttribute).IsAssignableFrom(attribute.GetType());
        }
    }
}

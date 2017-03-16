using System.Linq.Expressions;

namespace SimpleRules
{
    public class EvaluatedRule
    {
        public string MessageFormat { get; set; }
        public LambdaExpression Expression { get; set; }
    }
}

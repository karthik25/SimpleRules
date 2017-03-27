using System;
using System.Linq.Expressions;

namespace SimpleRules
{
    public class EvaluatedRule
    {
        private Delegate _delegate;

        public string MessageFormat { get; set; }
        public LambdaExpression Expression { get; set; }
        public RuleType RuleType { get; set; }
        public Delegate Delegate
        {
            get
            {
                if (_delegate == null)
                    _delegate = Expression.Compile();
                return _delegate;
            }
        }
    }
}

using System;
using System.Linq.Expressions;

namespace SimpleRules.Generic
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class LessThanOrEqualToAttribute : RelationalOperatorAttribute
    {
        public LessThanOrEqualToAttribute(string otherProp, object constantValue = null, bool canBeNull = false, RuleType ruleType = RuleType.Error) : base(otherProp, constantValue, canBeNull, ruleType) { }

        public override ExpressionType SupportedType
        {
            get
            {
                return ExpressionType.LessThanOrEqual;
            }
        }
    }
}

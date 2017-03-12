using System;
using System.Linq.Expressions;

namespace SimpleRules.Generic
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class GreaterThanAttribute : RelationalOperatorAttribute
    {
        public GreaterThanAttribute(string otherProp, object constantValue = null, bool canBeNull = false) : base(otherProp, constantValue, canBeNull) { }

        public override ExpressionType SupportedType
        {
            get
            {
                return ExpressionType.GreaterThan;
            }
        }
    }
}

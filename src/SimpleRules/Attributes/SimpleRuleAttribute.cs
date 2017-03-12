using System.Linq.Expressions;

namespace SimpleRules.Attributes
{
    public abstract class SimpleRuleAttribute : BaseRuleAttribute
    {
        public abstract ExpressionType SupportedType { get; }
        public abstract string OtherPropertyName { get; }
        public abstract object ConstantValue { get; }
        public abstract bool CanBeNull { get; }
    }
}

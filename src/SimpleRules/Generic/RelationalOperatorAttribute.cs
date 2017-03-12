using SimpleRules.Attributes;

namespace SimpleRules.Generic
{
    public abstract class RelationalOperatorAttribute : SimpleRuleAttribute
    {
        private readonly string _otherProp;
        private readonly object _constantValue;
        private readonly bool _canBeNull;

        public RelationalOperatorAttribute(string otherProp, object constantValue = null, bool canBeNull = false)
        {
            _otherProp = otherProp;
            _constantValue = constantValue;
            _canBeNull = canBeNull;
        }

        public override string OtherPropertyName
        {
            get
            {
                return _otherProp;
            }
        }

        public override object ConstantValue
        {
            get
            {
                return _constantValue;
            }
        }

        public override bool CanBeNull
        {
            get
            {
                return true;
            }
        }
    }
}

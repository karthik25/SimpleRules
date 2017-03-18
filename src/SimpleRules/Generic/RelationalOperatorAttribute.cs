using System;
using SimpleRules.Attributes;

namespace SimpleRules.Generic
{
    public abstract class RelationalOperatorAttribute : SimpleRuleAttribute
    {
        private readonly string _otherProp;
        private readonly object _constantValue;
        private readonly bool _canBeNull;
        private readonly RuleType _ruleType;

        public RelationalOperatorAttribute(string otherProp, object constantValue = null, bool canBeNull = false, RuleType ruleType = RuleType.Error)
        {
            _otherProp = otherProp;
            _constantValue = constantValue;
            _canBeNull = canBeNull;
            _ruleType = ruleType;
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
                return _canBeNull;
            }
        }

        public override RuleType RuleType
        {
            get
            {
                return _ruleType;
            }
        }
    }
}

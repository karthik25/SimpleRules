using System.Text.RegularExpressions;

namespace SimpleRules.Attributes
{
    public abstract class RegexRuleAttribute : BaseRuleAttribute
    {
        public abstract string RegularExpression { get; }
    }
}

using SimpleRules.Attributes;

namespace SimpleRules.Generic
{
    public class MatchRegexAttribute : RegexRuleAttribute
    {
        private readonly string _regex;

        public MatchRegexAttribute(string regex)
        {
            _regex = regex;
        }

        public override string RegularExpression
        {
            get
            {
                return _regex;
            }
        }
    }
}

using System.Linq;
using System.Collections.Generic;

namespace SimpleRules
{
    public class ValidationResult
    {
        public ValidationResult()
        {
            this.Errors = new List<string>();
            this.Warnings = new List<string>();
        }

        public object Key { get; set; }
        public List<string> Errors { get; set; }
        public List<string> Warnings { get; set; }
        public bool IsError
        {
            get
            {
                return Errors.Any();
            }
        }
        public bool HasWarnings
        {
            get
            {
                return Warnings.Any();
            }
        }
        public bool HasOnlyWarnings
        {
            get
            {
                return HasWarnings && !IsError;
            }
        }
        public bool HasOnlyErrors
        {
            get
            {
                return IsError && !HasWarnings;
            }
        }

        public void Add(string message, RuleType ruleType)
        {
            if (ruleType == RuleType.Error)
                this.Errors.Add(message);
            else
                this.Warnings.Add(message);
        }
    }
}

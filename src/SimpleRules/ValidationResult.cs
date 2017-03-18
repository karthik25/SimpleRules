using System.Collections.Generic;
using System.Linq;

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
    }
}

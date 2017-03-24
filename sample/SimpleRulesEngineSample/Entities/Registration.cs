using System;
using SimpleRules.Generic;

namespace SimpleRulesEngineSample.Entities
{
    public class Registration
    {
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class RegistrationMetadata
    {
        public DateTime StartDate { get; set; }
        [GreaterThan("StartDate", canBeNull: true)]
        public DateTime? EndDate { get; set; }
    }
}

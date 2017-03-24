using SimpleRules.Generic;
using SimpleRulesEngineSample.Rules;
using System;

namespace SimpleRulesEngineSample.Entities
{
    public class Activity
    {
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Capacity { get; set; }
    }

    public class ActivityMetadata
    {
        public DateTime StartDate { get; set; }
        [GreaterThan("StartDate", canBeNull: true)]
        public DateTime? EndDate { get; set; }
        [RangeRule(10, 20)]
        public int Capacity { get; set; }
    }
}

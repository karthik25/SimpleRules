using System;
using SimpleRules.Generic;
using SimpleRulesEngineSample.Rules;
using SimpleRules.Attributes;

namespace SimpleRulesEngineSample.Entities
{
    public class Student
    {
        [EntityKey]
        [GreaterThan("", constantValue: 100)]
        public int Id { get; set; }
        [MatchRegex(@"(^\d{3}-?\d{2}-?\d{4}$|^XXX-XX-XXXX$)")]
        public string Ssn { get; set; }
        [EmailMatchRegex]
        public string EmailAddress { get; set; }
        [LessThan("StartDate")]
        public DateTime RegistrationDate { get; set; }
        public DateTime StartDate { get; set; }
        [LessThan("StartDate", canBeNull: true)]
        public DateTime? EndDate { get; set; }
        [LessThan("RegistrationDate")]
        [LessThan("StartDate")]
        [LessThan("EndDate")]
        public DateTime DateOfBirth { get; set; }
        [RangeRule(2, 4)]
        public int EnrolledCount { get; set; }
        [UsPhoneNumberRegex]
        public string Contact { get; set; }
    }
}

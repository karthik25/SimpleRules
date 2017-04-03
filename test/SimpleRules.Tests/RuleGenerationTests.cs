using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleRules.Attributes;
using SimpleRules.Exceptions;
using SimpleRules.Generic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleRules.Tests
{
    [TestClass]
    public class RuleGenerationTests
    {
        [TestMethod]
        public void CanGenerateSimpleRule()
        {
            var users = new List<User>
            {
                new User { Id = 1001, RegistrationDate = DateTime.Now, LastLoginDate = DateTime.Now.AddDays(-1) }
            };
            var results = new SimpleRulesEngine()
                                .RegisterMetadata<User, UserMetadata>()
                                .Validate<User>(users);
            Assert.IsNotNull(results);
            Assert.AreEqual(1, results.Count());
            var first = results.First();
            Assert.AreEqual(1001, first.Key);
            Assert.AreEqual(1, first.Errors.Count);
            Assert.AreEqual("Last Login Date should be Greater Than the Registration Date.", first.Errors[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(DuplicateMetadataRegistrationException))]
        public void CanThrowExceptionForDuplicateMetdataRegistration()
        {
            var users = new List<User>
            {
                new User { Id = 1001, RegistrationDate = DateTime.Now, LastLoginDate = DateTime.Now.AddDays(-1) }
            };
            var results = new SimpleRulesEngine()
                                .RegisterMetadata<User, UserMetadata>()
                                .RegisterMetadata<User, UserMetadata>()
                                .Validate<User>(users);
            Assert.IsNotNull(results);
            Assert.AreEqual(1, results.Count());
        }

        [TestMethod]
        public void CanGenerateSimpleRuleWithWarnings()
        {
            var users = new List<User>
            {
                new User { Id = 1001, RegistrationDate = DateTime.Now, LastLoginDate = DateTime.Now.AddDays(-1) }
            };
            var results = new SimpleRulesEngine()
                                .RegisterMetadata<User, UserMetadataWithWarning>()
                                .Validate<User>(users);
            Assert.IsNotNull(results);
            Assert.AreEqual(1, results.Count());
            var first = results.First();
            Assert.AreEqual(1001, first.Key);
            Assert.AreEqual(1, first.Warnings.Count);
            Assert.AreEqual("Last Login Date should be Greater Than the Registration Date.", first.Warnings[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(NotNullablePropertyException))]
        public void CanIdentifyNotNullablePropWithCanBeNullSetWithConstant()
        {
            var jobs = new List<JobWithoutNull>
           { 
                new JobWithoutNull { Capacity = 10 }
            };
            var validationResultsEnumerator = new SimpleRulesEngine()
                                            .Validate<JobWithoutNull>(jobs);
            var validationResults = validationResultsEnumerator.ToList();

        }

        [TestMethod]
        public void CanIdentifyNullablePropWithCanBeBullSetWithConstant()
        {
            var jobs = new List<JobWithNull>
            {
                new JobWithNull { Capacity = null }
            };
            var validationResults = new SimpleRulesEngine()
                                            .Validate<JobWithNull>(jobs);
            Assert.AreEqual(1, validationResults.Count());
            Assert.AreEqual(0, validationResults.First().Errors.Count());
            jobs = new List<JobWithNull>
            {
                new JobWithNull { Capacity = 25, MaxCapacity = 15 }
            };
            Assert.AreEqual(1, validationResults.Count());
            Assert.AreEqual(0, validationResults.First().Errors.Count());
            jobs = new List<JobWithNull>
            {
                new JobWithNull { Capacity = 40, MaxCapacity = 45 }
            };
            Assert.AreEqual(1, validationResults.Count());
            Assert.AreEqual(0, validationResults.First().Errors.Count());
        }

        [TestMethod]
        public void CanGenerateMessagesconsideringNullAndConstantStatus()
        {
            var engine = new SimpleRulesEngine();
            var jobs = new List<JobWithNull>
            {
                new JobWithNull { MaxCapacity = 15, Capacity = 40 }
            };
            var validationResults = engine.Validate<JobWithNull>(jobs);
            Assert.AreEqual(1, validationResults.Count());
            Assert.AreEqual("Capacity should be Less Than the Max Capacity. Or Capacity can be Less Than 30. It can also be null.", validationResults.First().Errors[0]);
            var jobsWithConst = new List<JobWithOnlyConstant>
            {
                new JobWithOnlyConstant { MaxCapacity = 15, Capacity = 40 }
            };
            var validationResultsConst = engine.Validate<JobWithOnlyConstant>(jobsWithConst);
            Assert.AreEqual(1, validationResultsConst.Count());
            Assert.AreEqual("Capacity should be Less Than the Max Capacity. Or Capacity can be Less Than 30.", validationResultsConst.First().Errors[0]);
            var jobsWithNull = new List<JobWithOnlyNull>
            {
                new JobWithOnlyNull { MaxCapacity = 15, Capacity = 40 }
            };
            var validationResultsNull = engine.Validate<JobWithOnlyNull>(jobsWithNull);
            Assert.AreEqual(1, validationResultsNull.Count());
            Assert.AreEqual("Capacity should be Less Than the Max Capacity. It can also be null.", validationResultsNull.First().Errors[0]);
        }

        [TestMethod]
        public void CanGenerateRegexRules()
        {
            var codes = new List<Code>
            {
                new Code { Value = "abc" }
            };
            var validationResults = new SimpleRulesEngine()
                                        .Validate<Code>(codes);
            Assert.AreEqual(1, validationResults.Count());
            Assert.AreEqual(1, validationResults.First().Errors.Count());
            Assert.AreEqual("Value does not match the expected format", validationResults.First().Errors[0]);
            codes = new List<Code>
            {
                new Code { Value = "123" }
            };
            validationResults = new SimpleRulesEngine()
                                        .Validate<Code>(codes);
            Assert.AreEqual(1, validationResults.Count());
            Assert.AreEqual(0, validationResults.First().Errors.Count());
        }

        [TestMethod]
        public void CanIdentifyNullableOtherProperty()
        {
            var students = new List<Student>
            {
                new Student
                {
                    StartDate = DateTime.Now.AddDays(-5),
                    EndDate = DateTime.Now,
                    DateOfBirth = DateTime.Now.AddDays(1)
                }
            };
            var validationResults = new SimpleRulesEngine()
                                        .Validate<Student>(students);
            Assert.AreEqual(1, validationResults.Count());
            var first = validationResults.First();
            Assert.AreEqual(1, first.Errors.Count());
            Assert.AreEqual("Date Of Birth should be Less Than the End Date.", first.Errors[0]);
        }
    }

    public class Student
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        [LessThan("EndDate")]
        public DateTime DateOfBirth { get; set; }
    }

    public class User
    {
        public int Id { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime LastLoginDate { get; set; }
    }

    public class UserMetadata
    {
        [EntityKey]
        public int Id { get; set; }

        public DateTime RegistrationDate { get; set; }

        [GreaterThan("RegistrationDate")]
        public DateTime LastLoginDate { get; set; }
    }

    public class UserMetadataWithWarning
    {
        [EntityKey]
        public int Id { get; set; }

        public DateTime RegistrationDate { get; set; }

        [GreaterThan("RegistrationDate", ruleType: RuleType.Warning)]
        public DateTime LastLoginDate { get; set; }
    }

    public class Course
    {
        [EntityKey]
        public int Id { get; set; }
    }

    public class JobWithNull
    {
        [LessThan("MaxCapacity", canBeNull: true, constantValue: 30)]
        public int? Capacity { get; set; }
        public int MaxCapacity { get; set; }
    }

    public class JobWithoutNull
    {
        [LessThan("MaxCapacity", canBeNull: true, constantValue: 30)]
        public int Capacity { get; set; }
        public int MaxCapacity { get; set; }
    }

    public class JobWithOnlyConstant
    {
        [LessThan("MaxCapacity", constantValue: 30)]
        public int? Capacity { get; set; }
        public int MaxCapacity { get; set; }
    }

    public class JobWithOnlyNull
    {
        [LessThan("MaxCapacity", canBeNull: true)]
        public int? Capacity { get; set; }
        public int MaxCapacity { get; set; }
    }

    public class Code
    {
        [MatchRegex(@"\d+")]
        public string Value { get; set; }
    }
}

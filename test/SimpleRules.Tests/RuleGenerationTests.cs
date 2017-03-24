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
            Assert.AreEqual("Last Login Date should be Greater Than the Registration Date", first.Errors[0]);
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
            Assert.AreEqual("Last Login Date should be Greater Than the Registration Date", first.Warnings[0]);
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

    public class Code
    {
        [MatchRegex(@"\d+")]
        public string Value { get; set; }
    }
}

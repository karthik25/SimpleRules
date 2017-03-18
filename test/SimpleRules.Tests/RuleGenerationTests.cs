using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleRules.Attributes;
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
            Assert.AreEqual("Last Login Date should be greater than the Registration Date", first.Errors[0]);
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
            Assert.AreEqual("Last Login Date should be greater than the Registration Date", first.Warnings[0]);
        }

        [TestMethod]
        public void CanGenerateRegexRules()
        {

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
}

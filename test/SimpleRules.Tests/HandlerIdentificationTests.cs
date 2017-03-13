using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleRules.Attributes;
using SimpleRules.Contracts;
using SimpleRules.Exceptions;
using SimpleRules.Generic;
using SimpleRules.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SimpleRules.Tests
{
    [TestClass]
    public class HandlerIdentificationTests
    {
        [TestMethod]
        public void CanIdentifyAppropriateHandler()
        {
            var dictionary = new Dictionary<Type, IHandler>();
            dictionary.Add(typeof(RelationalOperatorAttribute), new SimpleRuleHandler());

            var type = typeof(Employee);
            var ruleAttributes = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                     .SelectMany(p => p.GetCustomAttributes<BaseRuleAttribute>())
                                     .ToList();
            ruleAttributes.ForEach(attr =>
            {
                var handler = dictionary.FindHandler(attr);
                Assert.IsNotNull(handler);
                Assert.AreEqual(typeof(SimpleRuleHandler), handler.GetType());
            });
        }

        [TestMethod]
        [ExpectedException(typeof(HandlerNotFoundException))]
        public void CanThrowExceptionsForUnkownAttributes()
        {
            var dictionary = new Dictionary<Type, IHandler>();
            var type = typeof(Employee);
            var ruleAttributes = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                     .SelectMany(p => p.GetCustomAttributes<BaseRuleAttribute>())
                                     .ToList();
            var handler = dictionary.FindHandler(ruleAttributes.First());
        }
    }
}

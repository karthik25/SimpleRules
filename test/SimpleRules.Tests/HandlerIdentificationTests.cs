using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleRules.Attributes;
using SimpleRules.Contracts;
using SimpleRules.Exceptions;
using SimpleRules.Generic;
using SimpleRules.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
            var type = typeof (Employee);
            var ruleAttributes = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                     .SelectMany(p => p.GetCustomAttributes<BaseRuleAttribute>())
                                     .ToList();
            var handler = dictionary.FindHandler(ruleAttributes.First());
        }

        [TestMethod]
        public void CanGenerateEvaluatedRuleForGreaterThanAttribute()
        {
            var handler = new SimpleRuleHandler();
            var targetProp = typeof (Registration).GetProperty("EndDate");
            var greaterThanAttribute = new GreaterThanAttribute("StartDate");
            var evaluatedRule = handler.GenerateEvaluatedRule<Registration>(greaterThanAttribute, targetProp);
            Assert.IsNotNull(evaluatedRule);
            Assert.AreEqual("End Date should be Greater Than the Start Date", evaluatedRule.MessageFormat);
            Assert.IsNotNull(evaluatedRule.Expression);
        }

        [TestMethod]
        public void CanIdentifyCustomHandlersRegistered()
        {
            var activities = new List<Activity>
            {
                new Activity { Id = 1, StartDate = DateTime.Parse("04/01/2017"), EndDate = DateTime.Parse("04/30/2017"), Capacity = 45, Name = "Cricket" }
            };
            var validationResults = new SimpleRulesEngine()
                                        .RegisterCustomRule<RangeRuleAttribute, RangeRuleHandler>()
                                        .Validate<Activity>(activities);
            Assert.AreEqual(1, validationResults.Count());
            var validationResult = validationResults.First();
            Assert.AreEqual(1, validationResult.Errors.Count);
            Assert.AreEqual("Capacity should be between 10 and 30", validationResult.Errors[0]);
        }
    }

    public class Registration
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class Activity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        [RangeRule(10, 30)]
        public int Capacity { get; set; }
    }

    public class RangeRuleAttribute : BaseRuleAttribute
    {
        public RangeRuleAttribute(int minValue, int maxValue)
        {
            MinValue = minValue;
            MaxValue = maxValue;
        }

        public int MinValue { get; set; }
        public int MaxValue { get; set; }
    }

    public class RangeRuleHandler : IHandler
    {
        public EvaluatedRule GenerateEvaluatedRule<T>(BaseRuleAttribute attribute, PropertyInfo targetProp)
        {
            var rangeAttr = attribute as RangeRuleAttribute;
            var input = Expression.Parameter(typeof(T), "a");
            var propName = targetProp.Name;
            var comparison = Expression.And(
                    Expression.GreaterThan(Expression.PropertyOrField(input, propName), Expression.Constant(rangeAttr.MinValue)),
                    Expression.LessThan(Expression.PropertyOrField(input, propName), Expression.Constant(rangeAttr.MaxValue))
                );
            var lambda = Expression.Lambda(comparison, input);
            return new EvaluatedRule
            {
                MessageFormat = string.Format("{0} should be between {1} and {2}", propName, rangeAttr.MinValue, rangeAttr.MaxValue),
                RuleType = RuleType.Error,
                Expression = lambda
            };
        }

        public bool Handles(BaseRuleAttribute attribute)
        {
            return typeof(RangeRuleAttribute).IsAssignableFrom(attribute.GetType());
        }
    }
}

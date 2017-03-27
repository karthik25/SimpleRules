# SimpleRules.Net

SimpleRules.Net is a rules engine, to be plain and simple, as you guessed probably based on the name! You may ask, why do we need another rules engine? Answer is, SimpleRules.Net was born out of a discussion I had at my workplace. The idea was to come up with a library that does not require me or anybody to write a gazillion lines of code and this led to the development of SimpleRules.Net. In order to define rules for a certain class or instance, all you have to do is decorate the class with pre-defined rule attributes. The basic usage section will get you started. And, it does not end there! Check out the sections after the basic usage section to know more!

### Basic Usage

Lets say you have a User object and you need to validate if the `Password` property matches the `ConfirmPassword` property, `EmailAddress` and `PhoneNumber` properties match their appropriate pattern. Here is what you could do:

```csharp
public class User
{
    public string Username { get; set; }
    public string Password { get; set; }
    [EqualTo("Password")]
    public string ConfirmPassword { get; set; }
    [EmailMatchRegex]
    public string EmailAddress { get; set; }
    [UsPhoneNumberRegex]
    public string PhoneNumber { get; set; }
}
```

As evident from the snippet above, all you had to do was decorate the `ConfirmPassword` property with the `EqualTo` attribute, by passing in the name of the property that has to be matched with, in this case `Password`. And for the `EmailAddress` and `PhoneNumber` properties, use the in-built `EmailMatchRegex` and `UsPhoneNumberRegex` properties! With this done, when you have an instance of user, it can be validated as shown below:

```csharp
var user = new User { 
     Username = "jdoe", 
     Password = "john", 
     ConfirmPassword = "johndoe", 
     EmailAddress = "abc", 
     PhoneNumber = "123" 
};
var simpleRulesEngine = new SimpleRulesEngine();
var result = simpleRulesEngine.Validate(new List<User> { user });
```

The result will be a `ValidationResult` that contains an `Errors` property with 3 errors in this case - one each for mismatching passwords, invalid email address and invalid phone number. That's it! It's that simple! In the next section, you will see how the rule metadata can be seperated out of the class to keep the entities and the rules separate!

### Specify Metadata Using an External Class

In the earlier section you saw how easy it is to setup and run a validation using the SimpleRules engine. In this section, we are going to see how the rules can be declared externally! To do this, just create a metadata class called `UserMetadata` and decorate your class with the `RuleMetadata` attribute, as shown below:

```csharp
[RuleMetadata(typeof(UserMetadata))]
public class User
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
    public string EmailAddress { get; set; }
    public string PhoneNumber { get; set; }
}

public class UserMetadata
{
    public string Username { get; set; }
    public string Password { get; set; }
    [EqualTo("Password")]
    public string ConfirmPassword { get; set; }
    [EmailMatchRegex]
    public string EmailAddress { get; set; }
    [UsPhoneNumberRegex]
    public string PhoneNumber { get; set; }
}
```

When you make the same call to `Validate` as before with a list of users, you will get the same results as before, an `Errors` property with 3 errors! There is more, move on to the next topic!

### Specify Metadata During Setup

Let's say you don't want to specify the metadata class in the class you want to validate. In that case, you just have to use the `RegisterMetadata` extension to register the metadata for a class, as shown below:

```csharp
var user = new User { 
     Username = "jdoe", 
     Password = "john", 
     ConfirmPassword = "johndoe", 
     EmailAddress = "abc", 
     PhoneNumber = "123" 
};
var simpleRulesEngine = new SimpleRulesEngine()
                            .RegisterMetadata<User, UserMetadata>();
var result = simpleRulesEngine.Validate(new List<User> { user });
```

The result of the validation is just the same as discussed before! Okay, we have seen enough of validation. Lets move on to possibilities of extending the rules engine using custom rules. That's next!

### Creating Custom Rules

SimpleRules.Net supports extensibility with the help of "handlers" to define new rules. That was one of the most important design considerations for me - to support the ability to define custom rules easily. As you probably guessed this rule engine is based on expressions. So to get futher with the handlers, you need to have a working knowledge of expressions. Let me try to explain the ability to create custom rules with the help of a rule that validates if a certain property is between a certain minimum and maximum value. In order to do this, first you need to create the attribute that contains the metadata for the rule - `RangeRuleAttribute`.

```csharp
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
```

As evident from the snippet above, the rule attribute defines a `MinValue` and a `MaxValue` to get the necessary metadata for the rule. The next step is to define a handler that implements the `IHandler` interface, as shown below:

```csharp
public class RangeRuleHandler : IHandler
{
    public EvaluatedRule GenerateEvaluatedRule<TConcrete>(BaseRuleAttribute attribute, PropertyInfo targetProp)
    {
        var rangeAttr = attribute as RangeRuleAttribute;
        var input = Expression.Parameter(typeof(TConcrete), "a");
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
```

The `Handles` method simply returns a boolean to indicate that this handler infact deals with a `RangeRuleAttribute`. And the `GenerateEvaluatedRule` method constructs an expression which uses the rule metadata to construct an expression tree that evaluates the same. In order to explain the expression tree generated by this method, let me define a class that will use this rule.

```csharp
public class Activity
{
    public int Id { get; set; }
    public string Name { get; set; }
    [RangeRule(10, 30)]
    public int Capacity { get; set; }
}
```

The `Func` given below explains the intended effect I wish to achieve. Given an actity, check if the property `Capacity` is between the numbers 10 and 30. That's it!

```csharp
Func<Activity, boo> func = a => a.Capacity > 10 && a.Capacity < 30;
```

The return value of the `GenerateEvaluatedRule` method needs to return an object of type `EvaluatedRule` which contains the evaluated rule itself along with other additional properties to help the core rules engine decide on things, like the message to be displayed if the rule is not met and whether its an error or a warning.

### Discover Handlers Dynamically Using Assembly Markers

ToDo

### Create new Rules for Existing Handlers

ToDo

### Usage in a .Net Core MVC Project

ToDo

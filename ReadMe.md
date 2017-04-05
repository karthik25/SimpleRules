# SimpleRules.Net

SimpleRules.Net is a rules engine, as you guessed probably based on the name! SimpleRules.Net was born out of a discussion I had at my workplace. The idea was to come up with a library that does not require me or anybody to write a gazillion lines of code to validate a set of instances (a `List<T>` or T specifically) and this led to the development of SimpleRules.Net. In order to define rules for a certain class or instance, all you have to do is decorate the class with pre-defined rule attributes. The basic usage section will get you started. And, it does not end there! Check out the sections after the basic usage section to know more!

SimpleRules.Net can be used in console, web applications or anything else for that matter. The sample included uses an MVC project to demonstrate how to use this library. SimpleRules.Net is NOT intended to be a replacement for the data annotations features that MVC provides, which are part of the `System.ComponentModel.DataAnnotations` namespace see [Using data annotations](https://docs.microsoft.com/en-us/aspnet/mvc/overview/older-versions/mvc-music-store/mvc-music-store-part-6). 

### Terminology

Before we get on with anything, I would like to define some terminology that will be used across this file. Consider the snippet below:

```csharp
public class User
{
    public string Password { get; set; }

    [EqualTo("Password")]
    public string ConfirmPassword { get; set; }
}
```
In the above class the property that is decorated with the `EqualTo` attribute will be referred to as the **"source"** and the property identified by the argument to this attribute (`Password` in this case) will be referred to as the **"target"**. For any source there could be multiple targets (i.e. rules).

### Basic Usage

Lets say you have a User object and you need to validate if the `Password` property matches the `ConfirmPassword` property and also if the `EmailAddress` and `PhoneNumber` properties match their appropriate pattern, as suggested by their names. Here is what you could do:

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

As evident from the snippet above, all you had to do was decorate the `ConfirmPassword` property with the `EqualTo` attribute, by passing in the name of the property that has to be matched with, in this case `Password`. And for the `EmailAddress` and `PhoneNumber` properties, use the in-built `EmailMatchRegex` and `UsPhoneNumberRegex` attributes! With this done, when you have an instance of user, it can be validated as shown below:

```csharp
var user = new User { 
     Username = "jdoe", 
     Password = "john", 
     ConfirmPassword = "johndoe", 
     EmailAddress = "abc", 
     PhoneNumber = "123" 
};
var simpleRulesEngine = new SimpleRulesEngine();
var result = simpleRulesEngine.Validate(user);
```

Needless to say, you can also pass in a list in order to validate a list of `User` objects (`List<User>`). An illustration is shown below:

```csharp
var users = ...; // Method that returns a list of users: List<User>
var simpleRulesEngine = new SimpleRulesEngine();
var results = simpleRulesEngine.Validate(users);
```

The result will be a `ValidationResult` that contains an `Errors` property with 3 errors in this case - one each for mismatching passwords, invalid email address and invalid phone number. That's it! It's that simple! In the next section, you will see how the rule metadata can be seperated out of the class to keep the entities and the rules separate!

One important thing to note is that when a rule is applied on a certain property (the source), the data types of the "target" property (or properties) should match that of the source. This is applicable when validating against a constant too. Simply, if the `EqualTo` attribute is applied on a `DateTime` property, the "target" property should also be a `DateTime` (or `DateTime?`).

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

As of now the rule metadata class has to contain the same set of properties (or more) that are present in the class that you intend to validate (`User`). And with respect to the data types, it can also be just an `object`. When you make the same call to `Validate` as before with a list of users, you will get the same results as before, an `Errors` property with 3 errors! There is more, move on to the next topic!

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
var result = simpleRulesEngine.Validate(user);
```

The result of the validation is just the same as discussed before! Okay, we have seen enough of validation. Note that if you attempt to register the same concrete type, metadata type again, it will throw a `DuplicateMetadataRegistrationException` exception. Lets move on to possibilities of extending the rules engine using custom rules. That's next!

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
            MessageFormat = string.Format("{0} should be between {1} and {2}", propName.AddSpaces(), rangeAttr.MinValue, rangeAttr.MaxValue),
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

The return value of the `GenerateEvaluatedRule` method needs to return an object of type `EvaluatedRule` which contains the evaluated rule itself along with other additional properties to help the core rules engine decide on things, like the message to be displayed if the rule is not met and whether its an error or a warning. If you notice the line where the message is constructed, I have used the extension method `AddSpaces` to make the property names readable, rather than being a string of text.  For example `StartDate` will be transformed to "Start Date" and so on. Its not over once you create the rule attribute and the handler. You need to register it with the rules engine. More on this is in the next section.

### Discover Handlers Dynamically Using Assembly Markers

In the previous section you saw have new rules/handlers can be defined. In order to put them to use, there are couple of ways. First one is by registering it with the rules engine as shown below:

```csharp
var activities = new List<Activity> { new Activity { Id = 1, Name = "Indoor", Capcaity = 45 } };
var engine = new SimpleRulesEngine()
                .RegisterCustomRule<RangeRuleHandler>();
var results = engine.Validate<Activity>(acitivities);
```

This might get tedious if you have multiple handlers defined. So, in order to let the rules engine automatically discover the handlers defined, use the `DiscoverHandlers` method, as shown below:

```csharp
var engine = new SimpleRulesEngine()
                .DiscoverHandlers(new[] { typeof(Marker) });                
```

`Marker` is simply a class that exists in the assembly that contains the defined handlers. With this method called, the handlers are all automatically discovered and registered and note that during the life cycle of the rules engine instance it may be called any number of times (handlers already registered will be ignored).

### Create new Rules for Existing Handlers

Its also possible to extend existing rules in order to support reuse. For example, consider the `MatchRegexRule`. This can be used to validate the value of a property against a regular expression. There are already rules like the `EmailMatchRegexRule`, `UsPhoneNumberRegex` etc to validate properties, but you can create your own rules based on this too. For example, the following code creates a rule that validates a password. I lifted the password validation regex straight out of google and it validates if the "password matching expression. match all alphanumeric character and predefined wild characters. password must consists of at least 8 characters and not more than 15 characters."

```csharp
public class PasswordMatchRegexAttribute : MatchRegexAttribute
{
    public PasswordMatchRegexAttribute()
        : base(@"^([a-zA-Z0-9@*#]{8,15})$")
    {
    }
}
```

With this rule you can validate a class that contains this rule decorated on a particular property.

### Multiple Validation Rules

You can also decorate a property with multiple rule attributes in order to validate it against a number of other properties. A sample is shown below:

```csharp
public class Student
{
    [EntityKey]
    [GreaterThan("", constantValue: 100)]
    public int Id { get; set; }
    
    [LessThan("StartDate")]
    public DateTime RegistrationDate { get; set; }
    
    public DateTime StartDate { get; set; }

    [LessThan("StartDate", canBeNull: true)]
    public DateTime? EndDate { get; set; }

    [LessThan("RegistrationDate")]
    [LessThan("StartDate")]
    [LessThan("EndDate")]
    public DateTime DateOfBirth { get; set; }
}
```

In the above snippet note the `DateOfBirth` property. It has been decorated with 3 validation rules to check whether it is less than 3 other properties: `RegistrationDate`, `StartDate` and `EndDate`.

Another thing of interest in the above snippet is the `EntityKey` attribute decorated on the `Id` property. You can use this attribute on a property to indicate that this value has to be returned as the value for the `Key` property in every `ValidationResult` instance returned with the results of the validation of a certain entity.

### Usage in a .Net Core MVC Project

The sample project provided in the solution has an example of how the rules engine can be used in a MVC project. This section provides a quick introduction of the same. In this case, the `Startup` class is used to create an instance of the rules engine as a singleton and configured in the `ConfigureServices` method, as shown below:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // ...
    var simpleRulesEngine = new SimpleRulesEngine()
                                .DiscoverHandlers(new [] { typeof(Startup) })
                                .RegisterMetadata<Registration, RegistrationMetadata>()
                                .RegisterMetadata<Activity, ActivityMetadata>();
    services.AddSingleton(typeof (SimpleRulesEngine), simpleRulesEngine);
    // ...
}
```

With this done, the SimpleRulesEngine can be injected in to any controller where you intend to do validation, like it is done in the case of the `HomeController`.

```csharp
public class HomeController : Controller
{
    private readonly SimpleRulesEngine _rulesEngine;

    public HomeController(SimpleRulesEngine rulesEngine)
    {
        _rulesEngine = rulesEngine;
    }

    // ...
}
```

### Contributing

If you would like to contribute to this project, please do :) There are no guidelines at this point, since its early and I need to come up with one :)

Happy coding!

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

### Create new Rules for Existing Handlers

ToDo

### Creating Custom Rules

ToDo

### Discover Handlers Dynamically Using Assembly Markers

ToDo

### Usage in a .Net Core MVC Project

ToDo

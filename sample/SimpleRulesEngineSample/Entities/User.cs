using SimpleRules.Generic;

namespace SimpleRulesEngineSample.Entities
{
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
}

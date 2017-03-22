namespace SimpleRules.Generic
{
    public class UsPhoneNumberRegexAttribute : MatchRegexAttribute        
    {
        public UsPhoneNumberRegexAttribute()
            : base(@"^\([2-9]{3}\)\s?[0-9]{3}-[0-9]{4}$")
        {
        }
    }
}

namespace SimpleRules.Generic
{
    public class EmailMatchRegexAttribute : MatchRegexAttribute
    {
        public EmailMatchRegexAttribute()
            : base(@"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z")
        {
        }
    }
}

using System;

namespace SimpleRules.Exceptions
{
    public class NotNullablePropertyException : Exception        
    {
        public NotNullablePropertyException(string propName)
            : base($"Property {propName} is not nullable. So canBeNull cannot be set to true")
        {
        }
    }
}

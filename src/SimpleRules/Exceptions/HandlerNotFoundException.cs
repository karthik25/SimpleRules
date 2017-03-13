using System;

namespace SimpleRules.Exceptions
{
    public class HandlerNotFoundException : Exception
    {
        public HandlerNotFoundException(string attrName, string attrType)
            : base($"Unable to find a handler for the attribute {attrName} ({attrType})")
        {
        }
    }
}

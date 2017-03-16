using System;

namespace SimpleRules.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class RuleMetadataAttribute : Attribute
    {
        private readonly Type _type;

        public RuleMetadataAttribute(Type type)
        {
            _type = type;
        }

        public Type MetaFor
        {
            get
            {
                return _type;
            }
        }
    }
}

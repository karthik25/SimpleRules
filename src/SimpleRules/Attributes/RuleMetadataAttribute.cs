using System;

namespace SimpleRules.Attributes
{
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

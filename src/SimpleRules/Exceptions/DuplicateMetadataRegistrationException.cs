using System;

namespace SimpleRules.Exceptions
{
    public class DuplicateMetadataRegistrationException : Exception
    {
        public DuplicateMetadataRegistrationException(string typeName, string metaTypeName)
            : base($"An attempt to register {typeName}, {metaTypeName} again was detected")
        {

        }
    }
}

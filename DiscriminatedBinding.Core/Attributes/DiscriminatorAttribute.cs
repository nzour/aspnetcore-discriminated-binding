using System;
using static System.AttributeTargets;

namespace DiscriminatedBinding.Core.Attributes
{
    [AttributeUsage(validOn: Class | Interface | Struct)]
    public class DiscriminatorAttribute : Attribute
    {
        public string Property { get; }

        public DiscriminatorAttribute(string property)
        {
            Property = property;
        }
    }
}
using System;

namespace DiscriminatedBinding.Core.Attributes
{
    [AttributeUsage(validOn: AttributeTargets.Class)]
    public class DiscriminatorAttribute : Attribute
    {
        public string Property { get; }

        public DiscriminatorAttribute(string property)
        {
            Property = property;
        }
    }
}
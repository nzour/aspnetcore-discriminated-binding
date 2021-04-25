using System;

namespace DiscriminatedBinding.Core.Attributes
{
    [AttributeUsage(validOn: AttributeTargets.Class, AllowMultiple = true)]
    public class DiscriminatorCaseAttribute : Attribute
    {
        public string Key { get; }
        public Type Implementation { get; }

        public DiscriminatorCaseAttribute(string when, Type then)
        {
            Key = when;
            Implementation = then;
        }
    }
}
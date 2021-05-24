using System;
using static System.AttributeTargets;

namespace DiscriminatedBinding.Core.Attributes
{
    [AttributeUsage(validOn: Class | Interface | Struct, AllowMultiple = true)]
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
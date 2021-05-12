using System;
using DiscriminatedBinding.Core.Attributes;

namespace DiscriminatedBinding.Core.Exceptions
{
    public class NoDiscriminatorCasesProvidedException : Exception
    {
        public NoDiscriminatorCasesProvidedException(Type modelType) : base($"Model '{modelType.Name}' should have at least one '{nameof(DiscriminatorCaseAttribute)}', non was specified.")
        {
        }
    }
}

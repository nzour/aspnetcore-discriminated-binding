using System;

namespace DiscriminatedBinding.Core.Exceptions
{
    public class CouldNotResolveBinderForTypeException : Exception
    {
        public CouldNotResolveBinderForTypeException(Type type) : base($"Could not resolve model binder for type '{type.Name}'")
        {
            
        }
    }
}

using System;

namespace DiscriminatedBinding.Core.Exceptions
{
    // fixme: should be 
    public class DiscriminatorException : Exception
    {
        public DiscriminatorException(string message) : base(message)
        {
            
        }
    }
}
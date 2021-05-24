namespace DiscriminatedBinding.Core.Exceptions
{
    public class CouldNotReadDiscriminatorException : System.Exception
    {
        public CouldNotReadDiscriminatorException() : base("Could not read discriminator from request.")
        {
        }
    }
}

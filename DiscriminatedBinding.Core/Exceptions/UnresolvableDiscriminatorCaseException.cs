namespace DiscriminatedBinding.Core.Exceptions
{
    public class UnresolvableDiscriminatorCaseException : System.Exception
    {
        public UnresolvableDiscriminatorCaseException(string discriminator) : base($"Could not resolve discriminator '{discriminator}'.")
        {
        }
    }
}

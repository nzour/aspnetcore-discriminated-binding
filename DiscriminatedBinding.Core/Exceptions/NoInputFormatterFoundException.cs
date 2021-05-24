namespace DiscriminatedBinding.Core.Exceptions
{
    public class NoInputFormatterFoundException : System.Exception
    {
        public NoInputFormatterFoundException() : base("No input formatter could be resolved.")
        {
        }
    }
}

namespace Cos.Library.Exceptions
{
    public class CosAssertException : CosExceptionBase
    {
        public CosAssertException()
        {
        }

        public CosAssertException(string message)
            : base($"Assert exception: {message}.")
        {
        }
    }
}

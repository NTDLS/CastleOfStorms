namespace Cos.Library.Exceptions
{
    public class CosNullException : CosExceptionBase
    {
        public CosNullException()
        {
        }

        public CosNullException(string message)
            : base($"Null exception: {message}.")
        {
        }
    }
}
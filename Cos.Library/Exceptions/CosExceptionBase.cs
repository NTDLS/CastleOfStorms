namespace Cos.Library.Exceptions
{
    public class CosExceptionBase : Exception
    {

        public CosExceptionBase()
        {
        }

        public CosExceptionBase(string? message)
            : base(message)
        {
        }
    }
}

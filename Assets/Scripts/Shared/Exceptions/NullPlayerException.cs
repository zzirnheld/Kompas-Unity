using KompasCore.Exceptions;

namespace KompasCore.Exceptions
{
    public class NullPlayerException : KompasException
    {
        public NullPlayerException(string debugMessage, string message = null) 
            : base(debugMessage, message ?? debugMessage)
        { }
    }
}
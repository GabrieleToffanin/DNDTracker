using System.Runtime.CompilerServices;

namespace DNDTracker.Domain.Exceptions;

public class SpellUnavailableException : Exception
{
    public SpellUnavailableException() : base()
    {
        
    }
    
    public SpellUnavailableException(
        string message) : base(message)
    {
        
    }
    
    public SpellUnavailableException(
        string message,
        Exception innerException) : base(message, innerException)
    {
        
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void Throw(
        string message = null, 
        Exception innerException = null)
    {
        throw new SpellUnavailableException(message, innerException);
    }
}
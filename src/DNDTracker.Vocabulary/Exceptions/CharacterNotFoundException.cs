namespace DNDTracker.Vocabulary.Exceptions;

public class CharacterNotFoundException : Exception
{
    public CharacterNotFoundException()
    {
    }

    public CharacterNotFoundException(string message) : base(message)
    {
    }

    public CharacterNotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

namespace DNDTracker.Domain.Exceptions;

public class InvalidCampaignDataException : Exception
{
    public InvalidCampaignDataException() : base()
    {
        
    }

    public InvalidCampaignDataException(string message) : base(message)
    {
        
    }
    
    public InvalidCampaignDataException(string message, Exception innerException) : base(message, innerException)
    {
        
    }
}
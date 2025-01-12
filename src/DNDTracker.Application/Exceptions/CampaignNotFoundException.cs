namespace DNDTracker.Application.Exceptions;

public class CampaignNotFoundException : Exception
{
    public CampaignNotFoundException()
    {
        
    }

    public CampaignNotFoundException(string message) : base(message)
    {
        
    }
    
    public CampaignNotFoundException(string message, Exception innerException) : base(message, innerException)
    {
        
    }
}
namespace Application.Core.Domain.Exceptions;

public class UnathorizedException : Exception
{
    public UnathorizedException()
        : base()
    {
    }
    
    public UnathorizedException(string message)
        : base(message)
    {
    }
}
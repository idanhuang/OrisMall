namespace OrisMall.Core.Exceptions;

public class ForbiddenException : BusinessException
{
    public ForbiddenException(string message = "Access forbidden") 
        : base(message, "FORBIDDEN", 403)
    {
    }
}
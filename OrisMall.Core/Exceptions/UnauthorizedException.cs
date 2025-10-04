namespace OrisMall.Core.Exceptions;

public class UnauthorizedException : BusinessException
{
    public UnauthorizedException(string message = "Unauthorized access") 
        : base(message, "UNAUTHORIZED", 401)
    {
    }
}

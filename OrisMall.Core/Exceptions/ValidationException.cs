namespace OrisMall.Core.Exceptions;

public class ValidationException : BusinessException
{
    public Dictionary<string, string[]> Errors { get; }

    public ValidationException(string message, Dictionary<string, string[]> errors) 
        : base(message, "VALIDATION_ERROR", 400)
    {
        Errors = errors;
    }

    public ValidationException(string message) 
        : base(message, "VALIDATION_ERROR", 400)
    {
        Errors = new Dictionary<string, string[]>();
    }
}

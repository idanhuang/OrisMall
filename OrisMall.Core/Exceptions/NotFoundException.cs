namespace OrisMall.Core.Exceptions;

public class NotFoundException : BusinessException
{
    public string ResourceType { get; }
    public object ResourceId { get; }

    public NotFoundException(string resourceType, object resourceId) 
        : base($"{resourceType} with ID '{resourceId}' was not found.", "NOT_FOUND", 404)
    {
        ResourceType = resourceType;
        ResourceId = resourceId;
    }

    public NotFoundException(string message) 
        : base(message, "NOT_FOUND", 404)
    {
        ResourceType = "Resource";
        ResourceId = "Unknown";
    }
}

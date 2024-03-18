namespace AiDevs2Reloaded.Api.Exceptions;

public class MissingTaskException : Exception
{
    public MissingTaskException() : base("Failed to get task")
    {
    }
}

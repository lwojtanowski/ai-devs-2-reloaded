namespace AiDevs2Reloaded.Api.Exceptions;

public class MissingTokenException : Exception
{
    public MissingTokenException() : base("Failed to get token")
    {
    }
}

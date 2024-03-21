namespace AiDevs2Reloaded.Api.Exceptions;

public class MissingModerationResponseException : Exception
{
    public MissingModerationResponseException() : base("Failed to get moderation response")
    {
    }
}

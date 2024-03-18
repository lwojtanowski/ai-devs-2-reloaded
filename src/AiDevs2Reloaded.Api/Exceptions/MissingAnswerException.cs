namespace AiDevs2Reloaded.Api.Exceptions;

public class MissingAnswerException : Exception
{
    public MissingAnswerException() : base("Failed to get response for sent answer")
    {
    }
}

namespace AiDevs2Reloaded.Api.Exceptions;

public class MissingEmbeddingException : Exception
{
    public MissingEmbeddingException() : base("Failed to get embedding")
    {
    }
}
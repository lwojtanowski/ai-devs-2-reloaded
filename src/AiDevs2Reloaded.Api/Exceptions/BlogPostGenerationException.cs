namespace AiDevs2Reloaded.Api.Exceptions;

public class BlogPostGenerationException : Exception
{
    public BlogPostGenerationException() : base("Failed to generaiting blog post")
    {
    }
}

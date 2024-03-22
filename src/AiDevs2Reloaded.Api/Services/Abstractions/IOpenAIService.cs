namespace AiDevs2Reloaded.Api.Services.Abstractions;

public interface IOpenAIService
{
    Task<List<string>> GenerateBlogPostAsync(string input, CancellationToken cancellationToken = default);
    Task<string> VerifyAsync(string input, CancellationToken cancellationToken = default);
}

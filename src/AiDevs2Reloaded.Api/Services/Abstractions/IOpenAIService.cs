using Azure.AI.OpenAI;

namespace AiDevs2Reloaded.Api.Services.Abstractions;

public interface IOpenAIService
{
    Task<List<string>> GenerateBlogPostAsync(string input, CancellationToken cancellationToken = default);
    Task<string> VerifyAsync(string input, CancellationToken cancellationToken = default);
    Task<string> CompletationsAsync(string input, string context, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EmbeddingItem>> EmbeddingAsync(List<string> input, CancellationToken cancellationToken = default);
    Task<string> AudioToSpeechAsync(Stream stream, CancellationToken cancellationToken = default);
    Task<string> AddUserAsync(string input, CancellationToken cancellationToken = default);
    Task<string> CompletionsAsync(string system, string input, CancellationToken cancellationToken = default);
    Task<string> AnalyzeImageAsync(string system, string url, CancellationToken cancellationToken = default);
}

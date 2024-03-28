namespace AiDevs2Reloaded.Api.Services.Abstractions;

public interface IOpenAIService
{
    Task<List<string>> GenerateBlogPostAsync(string input, CancellationToken cancellationToken = default);
    Task<string> VerifyAsync(string input, CancellationToken cancellationToken = default);
    Task<string> GenerateAnswerAsync(string input, string context, CancellationToken cancellationToken = default);
    Task<ReadOnlyMemory<float>> EmbeddingAsync(string input, CancellationToken cancellationToken = default);
    Task<string> AudioToSpeechAsync(Stream stream, CancellationToken cancellationToken = default);
}

namespace AiDevs2Reloaded.Api.Services.Abstractions;

public interface IOpenAISemanticKernalService
{
    Task<string> KnowledgeTaskAsync(string question, CancellationToken cancellationToken = default);
}

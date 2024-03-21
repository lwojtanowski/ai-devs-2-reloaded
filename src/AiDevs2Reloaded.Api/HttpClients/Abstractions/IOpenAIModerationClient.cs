using AiDevs2Reloaded.Api.Contracts.OpenAI;

namespace AiDevs2Reloaded.Api.HttpClients.Abstractions;

public interface IOpenAIModerationClient
{
    Task<ModerationResponse> CheckContentAsync(string input, CancellationToken cancellationToken = default);
}
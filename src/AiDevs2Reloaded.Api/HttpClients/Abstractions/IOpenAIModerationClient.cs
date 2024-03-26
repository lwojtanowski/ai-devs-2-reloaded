using AiDevs2Reloaded.Api.Contracts.OpenAI;

namespace AiDevs2Reloaded.Api.HttpClients.Abstractions;

public interface IOpenAIModerationClient
{
    Task<ModerationResponse> CheckContentAsync(List<string> input, CancellationToken cancellationToken = default);
}
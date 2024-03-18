using AiDevs2Reloaded.Api.Contracts.AiDevs;

namespace AiDevs2Reloaded.Api.HttpClients.Abstractions;
public interface ITasksAiDevsClient
{
    Task<string> GetTokenAsync(string taskName, CancellationToken cancellationToken = default);
    Task<TaskResponse> GetTaskAsync(string token, CancellationToken cancellationToken = default);
    Task<AnswerResponse> SendAnswerAsync(string token, string answer, CancellationToken cancellationToken = default);
}

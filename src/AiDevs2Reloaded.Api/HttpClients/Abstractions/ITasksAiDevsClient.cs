﻿using AiDevs2Reloaded.Api.Contracts.AIDevs;

namespace AiDevs2Reloaded.Api.HttpClients.Abstractions;
public interface ITasksAiDevsClient
{
    Task<string> GetTokenAsync(string taskName, CancellationToken cancellationToken = default);
    Task<TaskResponse> GetTaskAsync(string token, CancellationToken cancellationToken = default);
    Task<dynamic> GetTaskAsync(string token, IEnumerable<KeyValuePair<string, string>>? body, CancellationToken cancellationToken = default);
    Task<AnswerResponse> SendAnswerAsync<T>(string token, T answer, CancellationToken cancellationToken = default);
    Task<Stream> GetFileAsync(string url, CancellationToken cancellationToken = default);
}

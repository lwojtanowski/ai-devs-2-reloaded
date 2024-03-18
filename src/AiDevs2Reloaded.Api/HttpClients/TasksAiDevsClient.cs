﻿using AiDevs2Reloaded.Api.Configurations;
using AiDevs2Reloaded.Api.Contracts.AiDevs;
using AiDevs2Reloaded.Api.Exceptions;
using AiDevs2Reloaded.Api.HttpClients.Abstractions;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace AiDevs2Reloaded.Api.HttpClients;

public class TasksAiDevsClient : ITasksAiDevsClient
{
    private readonly HttpClient _httpClient;
    private readonly AiDevs2ReloadedOptions _options;
    private readonly ILogger<TasksAiDevsClient> _logger;
    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public TasksAiDevsClient(HttpClient httpClient, IOptions<AiDevs2ReloadedOptions> options, ILogger<TasksAiDevsClient> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<TaskResponse> GetTaskAsync(string token, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);

        Uri uri = new($"task/{token}", UriKind.Relative);

        try
        {
            var response = await _httpClient.GetAsync(uri, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogInformation("Response from {AddressUri}: {Response}", uri, content);
                TaskResponse tokenResponse = JsonSerializer.Deserialize<TaskResponse>(content, _jsonSerializerOptions)!;
                return tokenResponse;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while getting token");
            throw;
        }

        throw new MissingTaskException();
    }

    public async Task<string> GetTokenAsync(string taskName, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(taskName);
        ArgumentException.ThrowIfNullOrWhiteSpace(_options.ApiKey);

        Uri uri = new($"token/{taskName}", UriKind.Relative);
        TokenRequest request = new(_options.ApiKey);

        try
        {
            var response = await _httpClient.PostAsJsonAsync(uri, request, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogInformation("Response from {AddressUri}: {Response}", uri, content);
                TokenResponse tokenResponse = JsonSerializer.Deserialize<TokenResponse>(content, _jsonSerializerOptions)!;
                return tokenResponse.Token;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while getting token");
            throw;
        }
        
        throw new MissingTokenException();
    }

    public async Task<AnswerResponse> SendAnswerAsync(string token, string answer, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);
        ArgumentException.ThrowIfNullOrWhiteSpace(answer);

        Uri uri = new($"answer/{token}", UriKind.Relative);

        AnswerRequest request = new(answer);

        try
        {
            var response = await _httpClient.PostAsJsonAsync(uri, request, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogInformation("Response from {AddressUri}: {Response}", uri, content);
                AnswerResponse answerResponse = JsonSerializer.Deserialize<AnswerResponse>(content, _jsonSerializerOptions)!;
                return answerResponse;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while getting token");
            throw;
        }

        throw new MissingAnswerException();
    }
}
using AiDevs2Reloaded.Api.Configurations;
using AiDevs2Reloaded.Api.Contracts.OpenAI;
using AiDevs2Reloaded.Api.Exceptions;
using AiDevs2Reloaded.Api.HttpClients.Abstractions;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace AiDevs2Reloaded.Api.HttpClients;

public class OpenAIModerationClient : IOpenAIModerationClient
{
    private const string _apiVersion = "v1";

    private readonly HttpClient _httpClient;
    private readonly ILogger<OpenAIModerationClient> _logger;
    private readonly OpenAiOptions _options;
    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public OpenAIModerationClient(HttpClient httpClient, IOptions<OpenAiOptions> options, ILogger<OpenAIModerationClient> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<ModerationResponse> CheckContentAsync(List<string> input, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(_options.ApiKey);

        Uri uri = new($"{_apiVersion}/moderations", UriKind.Relative);
        ModerationRequest request = new(input);

        HttpRequestMessage httpRequestMessage = new(HttpMethod.Post, uri)
        {
            Content = JsonContent.Create(request, options: _jsonSerializerOptions),
        };

        httpRequestMessage.Headers.Add("Authorization", $"Bearer {_options.ApiKey}");

        try
        {
            var response = await _httpClient.SendAsync(httpRequestMessage, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogInformation("Response from {AddressUri}: {Response}", uri, content);
                ModerationResponse moderiationResponse = JsonSerializer.Deserialize<ModerationResponse>(content, _jsonSerializerOptions)!;
                return moderiationResponse;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while checking input");
            throw;
        }

        throw new MissingModerationResponseException();
    }
}

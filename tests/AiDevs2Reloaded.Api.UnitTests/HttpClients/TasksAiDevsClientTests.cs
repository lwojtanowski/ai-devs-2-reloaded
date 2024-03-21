using AiDevs2Reloaded.Api.Configurations;
using AiDevs2Reloaded.Api.Contracts.AIDevs;
using AiDevs2Reloaded.Api.Exceptions;
using AiDevs2Reloaded.Api.HttpClients;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using RichardSzalay.MockHttp;
using System.Net;
using System.Net.Http.Json;

namespace AiDevs2Reloaded.Api.UnitTests.HttpClients;

public class TasksAiDevsClientTests
{
    private readonly ILogger<TasksAiDevsClient> _logger;
    private readonly IOptions<AiDevs2ReloadedOptions> _options;
    private readonly MockHttpMessageHandler _mockHttpMessageHandler = new();

    public TasksAiDevsClientTests()
    {
        _logger = Substitute.For<ILogger<TasksAiDevsClient>>();
        _options = Options.Create(new AiDevs2ReloadedOptions
        {
            BaseUrl = "http://test.com",
            ApiKey = "testApiKey"
        });
    }

    [Fact]
    public async Task GetTaskAsync_SuccessfulResponse_ReturnsTaskResponse()
    {
        // Arrange
        var token = "testToken";
        var taskResponse = new TaskResponse(0, "Test Task", "aidevs_123", null, null);

        _mockHttpMessageHandler
            .When($"{_options.Value.BaseUrl}/task/{token}")
            .Respond(HttpStatusCode.OK, JsonContent.Create(taskResponse));

        var httpClient = _mockHttpMessageHandler.ToHttpClient();
        httpClient.BaseAddress = new(_options.Value.BaseUrl);

        var tasksAiDevsClient = new TasksAiDevsClient(httpClient, _options, _logger);

        // Act
        var result = await tasksAiDevsClient.GetTaskAsync(token);

        // Assert
        result.Should().BeEquivalentTo(taskResponse);
    }

    [Fact]
    public async Task GetTaskAsync_UnsuccessfulResponse_ThrowsMissingTaskException()
    {
        // Arrange
        var token = "testToken";

        _mockHttpMessageHandler
           .When($"{_options.Value.BaseUrl}/task/{token}")
           .Respond(HttpStatusCode.Forbidden);

        var httpClient = _mockHttpMessageHandler.ToHttpClient();
        httpClient.BaseAddress = new(_options.Value.BaseUrl);

        var tasksAiDevsClient = new TasksAiDevsClient(httpClient, _options, _logger);

        // Act
        Func<Task> act = async () => await tasksAiDevsClient.GetTaskAsync(token);

        // Assert
        await act.Should().ThrowAsync<MissingTaskException>();
    }

    [Fact]
    public async Task GetTaskAsync_ExceptionThrown_ThrowsException()
    {
        // Arrange
        var token = "testToken";

        _mockHttpMessageHandler
           .When($"{_options.Value.BaseUrl}/task/{token}")
           .Throw(new Exception());

        var httpClient = _mockHttpMessageHandler.ToHttpClient();
        httpClient.BaseAddress = new(_options.Value.BaseUrl);

        var tasksAiDevsClient = new TasksAiDevsClient(httpClient, _options, _logger);

        // Act
        Func<Task> act = async () => await tasksAiDevsClient.GetTaskAsync(token);

        // Assert
        await act.Should().ThrowAsync<Exception>();
    }
}
using AiDevs2Reloaded.Api.Contracts.AIDevs;
using AiDevs2Reloaded.Api.Exceptions;
using AiDevs2Reloaded.Api.HttpClients.Abstractions;
using AiDevs2Reloaded.Api.Modules;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace AiDevs2Reloaded.Api.UnitTests.Modules;

public class HelloApiTaskTests
{
    [Fact]
    public async Task HelloApiTaskAsync_ValidInput_ShouldReturnOk()
    {
        // Arrange
        var tasksAiDevsClient = Substitute.For<ITasksAiDevsClient>();
        var token = "testToken";
        var taskResponse = new TaskResponse(0, "please return value of \"cookie\" field as answer", "aidevs_0dabc7da", null, null);
        var response = new AnswerResponse(0, "OK", "CORRECT");

        tasksAiDevsClient.GetTokenAsync("helloapi", Arg.Any<CancellationToken>()).Returns(token);
        tasksAiDevsClient.GetTaskAsync(token, Arg.Any<CancellationToken>()).Returns(taskResponse);
        tasksAiDevsClient.SendAnswerAsync(token, taskResponse.Cookie, Arg.Any<CancellationToken>()).Returns(response);

        // Act
        var result = await TaskModule.HelloApiTaskAsync(tasksAiDevsClient, CancellationToken.None);

        // Assert
        result.Should().BeOfType<Microsoft.AspNetCore.Http.HttpResults.Ok<AnswerResponse>>();
        var okResult = result as Microsoft.AspNetCore.Http.HttpResults.Ok<AnswerResponse>;
        okResult!.Value.Should().Be(response);
    }

    [Fact]
    public async Task HelloApiTaskAsync_GetTokenAsyncThrowsException_ShouldThrowMissingTokenException()
    {
        // Arrange
        var tasksAiDevsClient = Substitute.For<ITasksAiDevsClient>();
        tasksAiDevsClient.GetTokenAsync("helloapi", Arg.Any<CancellationToken>()).Throws(new MissingTokenException());

        // Act
        Func<Task> act = async () => await TaskModule.HelloApiTaskAsync(tasksAiDevsClient, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<MissingTokenException>();
    }

    [Fact]
    public async Task HelloApiTaskAsync_GetTaskAsyncThrowsException_ShouldThrowMissingTaskException()
    {
        // Arrange
        var tasksAiDevsClient = Substitute.For<ITasksAiDevsClient>();
        var token = "testToken";

        tasksAiDevsClient.GetTokenAsync("helloapi", Arg.Any<CancellationToken>()).Returns(token);
        tasksAiDevsClient.GetTaskAsync(token, Arg.Any<CancellationToken>()).Throws(new MissingTaskException());

        // Act
        Func<Task> act = async () => await TaskModule.HelloApiTaskAsync(tasksAiDevsClient, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<MissingTaskException>();
    }

    [Fact]
    public async Task HelloApiTaskAsync_SendAnswerAsyncThrowsException_ShouldThrowMissingAnswerException()
    {
        // Arrange
        var tasksAiDevsClient = Substitute.For<ITasksAiDevsClient>();
        var token = "testToken";
        var taskResponse = new TaskResponse(0, "please return value of \"cookie\" field as answer", "aidevs_0dabc7da", null, null);

        tasksAiDevsClient.GetTokenAsync("helloapi", Arg.Any<CancellationToken>()).Returns(token);
        tasksAiDevsClient.GetTaskAsync(token, Arg.Any<CancellationToken>()).Returns(taskResponse);
        tasksAiDevsClient.SendAnswerAsync(token, taskResponse.Cookie, Arg.Any<CancellationToken>()).Throws(new MissingAnswerException());

        // Act
        Func<Task> act = async () => await TaskModule.HelloApiTaskAsync(tasksAiDevsClient, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<MissingAnswerException>();
    }
}

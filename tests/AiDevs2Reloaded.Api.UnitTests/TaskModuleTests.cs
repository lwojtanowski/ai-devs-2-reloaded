using AiDevs2Reloaded.Api.HttpClients.Abstractions;
using AiDevs2Reloaded.Api.Modules;
using FluentAssertions;
using NSubstitute;

namespace AiDevs2Reloaded.Api.UnitTests;
public class TaskModuleTests
{
    //write unit tests for method HelloApiTaskAsync in TaskModule from AiDevs2Reloaded.Api
    //for that uste NSubstitute to mock ITasksAiDevsClient
    //use FluentAssertions to assert the result

    [Fact]
    public async Task GetHelloApiTask_ValidInput_ShouldReturnOk()
    {
        //Arrange
        var taslsAiDevsClient = Substitute.For<ITasksAiDevsClient>();

        //Act
        var result = await TaskModule.HelloApiTaskAsync(taslsAiDevsClient, CancellationToken.None);

        //Assert
        result.Should().NotBeNull();
    }
}
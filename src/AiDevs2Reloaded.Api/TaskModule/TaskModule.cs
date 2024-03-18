using AiDevs2Reloaded.Api.HttpClients.Abstractions;

namespace AiDevs2Reloaded.Api.TaskModule;

public static class TaskModule
{
    public static void AddTaskModule(this IEndpointRouteBuilder app)
    {
        app.MapGet("/helloapi", async (ITasksAiDevsClient client) =>
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(120));
            var token = await client.GetTokenAsync("helloapi", cts.Token);
            var task = await client.GetTaskAsync(token, cts.Token);
            var response = await client.SendAnswerAsync(token, task.Cookie, cts.Token);
            return Results.Ok(response);
        })
        .WithName("helloapi")
        .WithOpenApi();
    }
}

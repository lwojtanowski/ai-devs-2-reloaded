using AiDevs2Reloaded.Api.HttpClients.Abstractions;

namespace AiDevs2Reloaded.Api.Modules;

internal static class TaskModule
{
    internal static void AddTaskModule(this IEndpointRouteBuilder app)
    {
        app.MapGet("/helloapi", async (ITasksAiDevsClient client, CancellationToken ct) => await HelloApiTaskAsync(client, ct))
            .WithName("helloapi")
            .WithOpenApi();
    }

    internal static async Task<IResult> HelloApiTaskAsync(ITasksAiDevsClient client, CancellationToken cancellationToken = default)
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(120));
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, cts.Token);

        var token = await client.GetTokenAsync("helloapi", linkedCts.Token);
        var task = await client.GetTaskAsync(token, linkedCts.Token);
        var response = await client.SendAnswerAsync(token, task.Cookie, linkedCts.Token);
        return Results.Ok(response);
    }
}

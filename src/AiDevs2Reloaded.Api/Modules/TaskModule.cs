using AiDevs2Reloaded.Api.HttpClients.Abstractions;
using AiDevs2Reloaded.Api.Services.Abstractions;
using System.Text.Json.Nodes;

namespace AiDevs2Reloaded.Api.Modules;

internal static class TaskModule
{
    internal static void AddTaskModule(this IEndpointRouteBuilder app)
    {
        app.MapGet("/helloapi", async (ITasksAiDevsClient client, CancellationToken ct) => await HelloApiTaskAsync(client, ct))
            .WithName("helloapi")
            .WithOpenApi();

        app.MapGet("/moderate", async (ITasksAiDevsClient client, IOpenAIModerationClient openAIModClient, CancellationToken ct) =>
            await ModerateTaskAsync(client, openAIModClient, ct))
            .WithName("moderate")
            .WithOpenApi();

        app.MapGet("/blogger", async (IOpenAIService service, ITasksAiDevsClient client, CancellationToken ct) => await BloggerTaskAsync(service, client, ct))
            .WithName("blogger")
            .WithOpenApi();

        app.MapGet("/liar", async (IOpenAIService service, ITasksAiDevsClient client, CancellationToken ct) => await LiarTaskAsync(service, client, ct))
            .WithName("liar")
            .WithOpenApi();
    }

    internal static async Task<IResult> HelloApiTaskAsync(ITasksAiDevsClient client, CancellationToken cancellationToken = default)
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(120));
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, cts.Token);

        var token = await client.GetTokenAsync("helloapi", linkedCts.Token);
        var task = await client.GetTaskAsync(token, linkedCts.Token);
        var response = await client.SendAnswerAsync(token, task.Cookie!, linkedCts.Token);
        return Results.Ok(response);
    }

    internal static async Task<IResult> ModerateTaskAsync(
        ITasksAiDevsClient client,
        IOpenAIModerationClient openAIModClient,
        CancellationToken cancellationToken = default)
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(120));
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, cts.Token);

        var token = await client.GetTokenAsync("moderation", linkedCts.Token);
        var task = await client.GetTaskAsync(token, linkedCts.Token);

#pragma warning disable S125
        //Normally we would use Task.WhenAll to run all the tasks in parallel but I assume we need responses in order
        //var results = await Task.WhenAll(tasks);

        var results = new List<int>();

        foreach (var input in task.Input!)
        {
            var moderationResponse = await openAIModClient.CheckContentAsync(input, linkedCts.Token);
            results.Add(moderationResponse.Results.Any(x => x.Flagged) ? 1 : 0);
        }
#pragma warning restore S125

        var response = await client.SendAnswerAsync(token, results, linkedCts.Token);
        return Results.Ok(response);
    }

    internal static async Task<IResult> BloggerTaskAsync(IOpenAIService service, ITasksAiDevsClient client, CancellationToken cancellationToken)
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(120));
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, cts.Token);

        var token = await client.GetTokenAsync("blogger", linkedCts.Token);
        var task = await client.GetTaskAsync(token, linkedCts.Token);
        var chapters = string.Join(", ", task.Blog!);
        var input = "Write a blog post about pizza margherita with the following chapters: " + chapters;
        var result = await service.GenerateBlogPostAsync(input, linkedCts.Token);
        var response = await client.SendAnswerAsync(token, result, linkedCts.Token);
        return Results.Ok(response);
    }

    internal static async Task<IResult> LiarTaskAsync(IOpenAIService service, ITasksAiDevsClient client, CancellationToken cancellationToken)
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(120));
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, cts.Token);

        var token = await client.GetTokenAsync("liar", linkedCts.Token);
        var question = "What is capital of Poland?";
        var body = new List<KeyValuePair<string, string>>
        {
           new("question", question)
        };

        JsonObject task = await client.GetTaskAsync(token, body, linkedCts.Token);
        task.TryGetPropertyValue("answer", out var answer);
        var result = await service.VerifyAsync($"{question} {answer}", linkedCts.Token);
        var response = await client.SendAnswerAsync(token, result, linkedCts.Token);
        return Results.Ok(response);
    }
}

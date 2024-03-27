using AiDevs2Reloaded.Api.HttpClients.Abstractions;
using AiDevs2Reloaded.Api.Services.Abstractions;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

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

        app.MapGet("/inprompt", async (IOpenAIService service, ITasksAiDevsClient client, CancellationToken ct) => await InpromptTaskAsync(service, client, ct))
            .WithName("inprompt")
            .WithOpenApi();

        app.MapGet("/embedding", async (IOpenAIService service, ITasksAiDevsClient client, CancellationToken ct) => await EmbeddingTaskAsync(service, client, ct))
            .WithName("embedding")
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
        var moderationResponse = await openAIModClient.CheckContentAsync(task.Input!, linkedCts.Token);

        var results = moderationResponse.Results
            .Select(x => x.Flagged ? 1 : 0)
            .ToList();

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

    internal static async Task<IResult> InpromptTaskAsync(IOpenAIService service, ITasksAiDevsClient client, CancellationToken cancellationToken)
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(120));
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, cts.Token);

        var token = await client.GetTokenAsync("inprompt", linkedCts.Token);
        JsonObject task = await client.GetTaskAsync(token, null, linkedCts.Token);
        task.TryGetPropertyValue("input", out var input);

        var data = input!
            .AsArray()
            .Select(x => x!.GetValue<string>())
            .GroupBy(x => new Regex(@"^\s*\w+").Match(x).Value, x => x);

        task.TryGetPropertyValue("question", out var question);
        var name = new Regex(@"\w+(?=[.!?]?\s*$)").Match(question!.GetValue<string>()).Value;
        var userData = data.SingleOrDefault(x => x.Key.Equals(name, StringComparison.OrdinalIgnoreCase));

        if (userData is null)
        {
            throw new InvalidOperationException("User data not found");
        }

        var context = string.Join("; ", userData);

        //If you want save some monay just use this:
        //var response = await client.SendAnswerAsync(token, context, linkedCts.Token);
        //return Results.Ok(response);

        var answer = await service.GenerateAnswerAsync(question!.GetValue<string>(), context, linkedCts.Token);
        var response = await client.SendAnswerAsync(token, answer, linkedCts.Token);
        return Results.Ok(response);
    }

    internal static async Task<IResult> EmbeddingTaskAsync(IOpenAIService service, ITasksAiDevsClient client, CancellationToken cancellationToken)
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(120));
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, cts.Token);

        var token = await client.GetTokenAsync("embedding", linkedCts.Token);
        var answer = await service.EmbeddingAsync("Hawaiian pizza", linkedCts.Token);
        var response = await client.SendAnswerAsync(token, answer, linkedCts.Token);
        return Results.Ok(response);
    }
}

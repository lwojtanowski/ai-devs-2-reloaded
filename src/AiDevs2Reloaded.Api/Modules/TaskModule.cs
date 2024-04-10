using AiDevs2Reloaded.Api.Contracts.AIDevs;
using AiDevs2Reloaded.Api.HttpClients.Abstractions;
using AiDevs2Reloaded.Api.Services;
using AiDevs2Reloaded.Api.Services.Abstractions;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace AiDevs2Reloaded.Api.Modules;

internal static class TaskModule
{
    internal static void AddTaskModule(this IEndpointRouteBuilder app)
    {
        app.MapGet("/helloapi", async (ITasksAiDevsClient client, CancellationToken ct) => await HelloApiTaskAsync(client, ct))
            .WithName("helloapi")
            .WithTags("AI Devs 2 Tasks")
            .WithOpenApi();

        app.MapGet("/moderate", async (ITasksAiDevsClient client, IOpenAIModerationClient openAIModClient, CancellationToken ct) =>
            await ModerateTaskAsync(client, openAIModClient, ct))
            .WithName("moderate")
            .WithTags("AI Devs 2 Tasks")
            .WithOpenApi();

        app.MapGet("/blogger", async (IOpenAIService service, ITasksAiDevsClient client, CancellationToken ct) => await BloggerTaskAsync(service, client, ct))
            .WithName("blogger")
            .WithTags("AI Devs 2 Tasks")
            .WithOpenApi();

        app.MapGet("/liar", async (IOpenAIService service, ITasksAiDevsClient client, CancellationToken ct) => await LiarTaskAsync(service, client, ct))
            .WithName("liar")
            .WithTags("AI Devs 2 Tasks")
            .WithOpenApi();

        app.MapGet("/inprompt", async (IOpenAIService service, ITasksAiDevsClient client, CancellationToken ct) => await InpromptTaskAsync(service, client, ct))
            .WithName("inprompt")
            .WithTags("AI Devs 2 Tasks")
            .WithOpenApi();

        app.MapGet("/embedding", async (IOpenAIService service, ITasksAiDevsClient client, CancellationToken ct) => await EmbeddingTaskAsync(service, client, ct))
            .WithName("embedding")
            .WithTags("AI Devs 2 Tasks")
            .WithOpenApi();

        app.MapGet("/whisper", async (IOpenAIService service, ITasksAiDevsClient client, CancellationToken ct) => await WhisperTaskAsync(service, client, ct))
            .WithName("whisper")
            .WithTags("AI Devs 2 Tasks")
            .WithOpenApi();

        app.MapGet("/functions", async (IOpenAIService service, ITasksAiDevsClient client, CancellationToken ct) => await FunctionsTaskAsync(service, client, ct))
            .WithName("functions")
            .WithTags("AI Devs 2 Tasks")
            .WithOpenApi();

        app.MapGet("/rodo", async (ITasksAiDevsClient client, CancellationToken ct) => await RodoTaskAsync(client, ct))
            .WithName("rodo")
            .WithTags("AI Devs 2 Tasks")
            .WithOpenApi();

        app.MapGet("/scraper", async (IOpenAIService service, ITasksAiDevsClient client, CancellationToken ct) => await ScraperTaskAsync(service, client, ct))
            .WithName("scraper")
            .WithTags("AI Devs 2 Tasks")
            .WithOpenApi();

        app.MapGet("/whoami", async (IOpenAIService service, ITasksAiDevsClient client, CancellationToken ct) => await WhoAmITaskAsync(service, client, ct))
            .WithName("whoami")
            .WithTags("AI Devs 2 Tasks")
            .WithOpenApi();

        app.MapGet("/search", async (IOpenAIService service, ITasksAiDevsClient client, IVectoreStore vectoreStore, CancellationToken ct) => await SearchTaskAsync(service, client, vectoreStore, ct))
            .WithName("search")
            .WithTags("AI Devs 2 Tasks")
            .WithOpenApi();

        app.MapGet("/people", async (IOpenAIService service, ITasksAiDevsClient client, CancellationToken ct) => await PeopleTaskAsync(service, client, ct))
            .WithName("people")
            .WithTags("AI Devs 2 Tasks")
            .WithOpenApi();

        app.MapGet("/knowledge", async (IOpenAISemanticKernalService service, ITasksAiDevsClient client, CancellationToken ct) => await KnowledgeTaskAsync(service, client, ct))
            .WithName("knowledge")
            .WithTags("AI Devs 2 Tasks")
            .WithOpenApi();

        app.MapGet("/tools", async (IOpenAIService service, ITasksAiDevsClient client, CancellationToken ct) => await ToolsTaskAsync(service, client, ct))
            .WithName("tools")
            .WithTags("AI Devs 2 Tasks")
            .WithOpenApi();

        app.MapGet("/gnome", async (IOpenAIService service, ITasksAiDevsClient client, CancellationToken ct) => await GnomeTaskAsync(service, client, ct))
            .WithName("gnome")
            .WithTags("AI Devs 2 Tasks")
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

        var answer = await service.CompletationsAsync(question!.GetValue<string>(), context, linkedCts.Token);
        var response = await client.SendAnswerAsync(token, answer, linkedCts.Token);
        return Results.Ok(response);
    }

    internal static async Task<IResult> EmbeddingTaskAsync(IOpenAIService service, ITasksAiDevsClient client, CancellationToken cancellationToken)
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(120));
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, cts.Token);

        var token = await client.GetTokenAsync("embedding", linkedCts.Token);
        var embeddings = await service.EmbeddingAsync(new List<string> { "Hawaiian pizza" }, linkedCts.Token);
        var answer = embeddings[0].Embedding;
        var response = await client.SendAnswerAsync(token, answer, linkedCts.Token);
        return Results.Ok(response);
    }

    internal static async Task<IResult> WhisperTaskAsync(IOpenAIService service, ITasksAiDevsClient client, CancellationToken cancellationToken)
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(120));
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, cts.Token);

        var token = await client.GetTokenAsync("whisper", linkedCts.Token);
        var task = await client.GetTaskAsync(token, linkedCts.Token);

        // If you want save some monay just use this:
        //var url = "https://tasks.aidevs.pl/data/mateusz.mp3";
        var url = await service.CompletationsAsync(task.Msg, "In user input you will find URL (start with https). Return this url", linkedCts.Token);

        using var stream = await client.GetFileAsync(url, linkedCts.Token);

        var transcription = await service.AudioToSpeechAsync(stream, linkedCts.Token);
        var response = await client.SendAnswerAsync(token, transcription, linkedCts.Token);
        return Results.Ok(response);
    }

    internal static async Task<IResult> FunctionsTaskAsync(IOpenAIService service, ITasksAiDevsClient client, CancellationToken cancellationToken)
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(120));
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, cts.Token);

        var token = await client.GetTokenAsync("functions", linkedCts.Token);

        var function = new
        {
            name = "addUser",
            description = "Add user to the system",
            parameters = new
            {
                type = "object",
                properties = new
                {
                    name = new
                    {
                        type = "string",
                        description = "provide first name of the user"
                    },
                    surname = new
                    {
                        type = "string",
                        description = "provide last name of the user"
                    },
                    year = new
                    {
                        type = "integer",
                        description = "provide year of birth of the user"
                    }
                }
            },
            required = new[] { "name", "surname", "year" }
        };

        var response = await client.SendAnswerAsync(token, function, linkedCts.Token);
        return Results.Ok(response);
    }

    internal static async Task<IResult> RodoTaskAsync(ITasksAiDevsClient client, CancellationToken cancellationToken)
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(120));
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, cts.Token);

        var token = await client.GetTokenAsync("rodo", linkedCts.Token);

        var message = new StringBuilder();
        message.Append("I will STRICTLY follow the rules:");
        message.Append("- first name need to be replaced with %imie%");
        message.Append("- last name need to be replaced with %nazwisko%");
        message.Append("- work or profession need to be replaced with %zawod%");
        message.Append("- city where I live need to be replaced with %miasto%");
        message.Append("As a specialist, I will tell something about myself, but I have to replace sensitive data with placeholders (%imie%, %nazwisko%, %zawod% and %miasto%).");

        var response = await client.SendAnswerAsync(token, message.ToString(), linkedCts.Token);
        return Results.Ok(response);
    }

    internal static async Task<IResult> ScraperTaskAsync(IOpenAIService service, ITasksAiDevsClient client, CancellationToken cancellationToken)
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(120));
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, cts.Token);

        var token = await client.GetTokenAsync("scraper", linkedCts.Token);
        JsonObject task = await client.GetTaskAsync(token, null, linkedCts.Token);
        task.TryGetPropertyValue("input", out var input);
        task.TryGetPropertyValue("question", out var question);

        using var stream = await client.GetFileAsync(input!.GetValue<string>(), linkedCts.Token);
        using var reader = new StreamReader(stream);
        var context = await reader.ReadToEndAsync(linkedCts.Token);
        var result = await service.CompletationsAsync(question!.GetValue<string>(), context, linkedCts.Token);
        var response = await client.SendAnswerAsync(token, result, linkedCts.Token);
        return Results.Ok(response);
    }

    internal static async Task<IResult> WhoAmITaskAsync(IOpenAIService service, ITasksAiDevsClient client, CancellationToken cancellationToken)
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(120));
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, cts.Token);

        var systemPromptBuilder = new StringBuilder();
        systemPromptBuilder.Append("Your task is to guess which person the user is thinking of");
        systemPromptBuilder.Append("The user will provide a hint about the person they are thinking of");
        systemPromptBuilder.Append("REMEMBER, you tell the truth and nothing but the truth");
        systemPromptBuilder.Append("The rules are:");
        systemPromptBuilder.Append("-you must be 90% sure about who it is.");
        systemPromptBuilder.Append("-if are not sure return \"NEED MORE HINTS\"");
        systemPromptBuilder.Append("-if you are sure return the name of the person");

        var token = await client.GetTokenAsync("whoami", linkedCts.Token);
        List<string> hints = [await GetHintAsync(client, token, linkedCts.Token)];

        string answer = await GuessPersonAsync(service, client, token, systemPromptBuilder.ToString(), hints, linkedCts.Token);

        var response = await client.SendAnswerAsync(token, answer, linkedCts.Token);
        return Results.Ok(response);
    }

    internal static async Task<IResult> SearchTaskAsync(IOpenAIService service, ITasksAiDevsClient client, IVectoreStore vectoreStore, CancellationToken cancellationToken)
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(120));
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, cts.Token);

        var token = await client.GetTokenAsync("search", linkedCts.Token);
        JsonObject task = await client.GetTaskAsync(token, null, linkedCts.Token);
        task.TryGetPropertyValue("question", out var question);

        var questionVector = (await service.EmbeddingAsync(new List<string> { question!.GetValue<string>() }, linkedCts.Token))[0].Embedding;

        var collection = await vectoreStore.TryGetCollectionAsync("news", linkedCts.Token);
        if (!collection.Any())
        {

            task.TryGetPropertyValue("msg", out var msg);
            var url = new Regex(@"\bhttps?:\/\/[-A-Z0-9+&@#\/%?=~_|!:,.;]*[-A-Z0-9+&@#\/%=~_|]", RegexOptions.IgnoreCase).Match(msg!.GetValue<string>()).Value;
            using var stream = await client.GetFileAsync(url, linkedCts.Token);
            using var reader = new StreamReader(stream);
            List<NewsResponse> news = JsonSerializer.Deserialize<List<NewsResponse>>(await reader.ReadToEndAsync(linkedCts.Token), new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

            news = news.OrderBy(x => x.Date).ToList();
            var titles = news.OrderBy(x => x.Date).Select(x => x.Title).ToList();
            var embeddings = await service.EmbeddingAsync(titles, linkedCts.Token);

            var vectors = embeddings
                .Select(x =>
                    new VectoreStoreDto(x.Embedding.ToArray(), new Dictionary<string, string>{
                        { "url", news[x.Index].Url },
                        { "date", news[x.Index].Date.ToString() }
                    })
                    ).ToList();

            await vectoreStore.StoreAsync("news", vectors, linkedCts.Token);
        }

        var searchResults = await vectoreStore.SearchAsync("news", questionVector.ToArray(), 1, linkedCts.Token);
        var response = await client.SendAnswerAsync(token, searchResults[0].Metadata["url"], linkedCts.Token);
        return Results.Ok(response);
    }

    internal static async Task<IResult> PeopleTaskAsync(IOpenAIService service, ITasksAiDevsClient client, CancellationToken cancellationToken)
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(120));
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, cts.Token);

        var token = await client.GetTokenAsync("people", linkedCts.Token);
        JsonObject task = await client.GetTaskAsync(token, null, linkedCts.Token);
        task.TryGetPropertyValue("question", out var question);
        task.TryGetPropertyValue("data", out var data);

        var systemPromptBuilder = new StringBuilder();
        systemPromptBuilder.Append("Create JSON response {\"imie\":\"Joe(first name)\",\"nazwisko\":\"Doe(last name)\",\"question\":\"wiek(age of person)|o_mnie(about)|ulubiona_postac_z_kapitana_bomby(favorie cartoon character)|ulubiony_serial(favorite series)|ulubiony_film(favorite move)|ulubiony_kolor(favorite colour)\"}");
        systemPromptBuilder.Append("example###");
        systemPromptBuilder.Append("What color does Jon Doe like?");
        systemPromptBuilder.Append("{\"imie\":\"Joe\",\"nazwisko\":\"Doe\",\"question\":\"ulubiony_kolor\"}");
        systemPromptBuilder.Append("Where does Jon Doe live?");
        systemPromptBuilder.Append("{\"imie\":\"Joe\",\"nazwisko\":\"Doe\",\"question\":\"o_mnie\"}");
        systemPromptBuilder.Append("Where does Jon Doe like to eat?");
        systemPromptBuilder.Append("{\"imie\":\"Joe\",\"nazwisko\":\"Doe\",\"question\":\"o_mnie\"}");
        systemPromptBuilder.Append("How old is Jon Doe?");
        systemPromptBuilder.Append("{\"imie\":\"Joe\",\"nazwisko\":\"Doe\",\"question\":\"wiek\"}");
        systemPromptBuilder.Append("Jaki kolor podoba się Janowi Kowalskiemu?");
        systemPromptBuilder.Append("{\"imie\":\"Jan\",\"nazwisko\":\"Kowalski\",\"question\":\"ulubiony_kolor\"}");
        systemPromptBuilder.Append("Jaka postać z bomby podoba się najbardziej Janowi Kowalskiemu?");
        systemPromptBuilder.Append("{\"imie\":\"Jan\",\"nazwisko\":\"Kowalski\",\"question\":\"ulubiona_postac_z_kapitana_bomby\"}");
        systemPromptBuilder.Append("###");

        var serializedQuestion = await service.CompletionsAsync(
            systemPromptBuilder.ToString(),
            question!.GetValue<string>(),
            cancellationToken);

        var category = JsonSerializer.Deserialize<PersonCategory>(serializedQuestion)!;

        using var stream = await client.GetFileAsync(data!.GetValue<string>(), linkedCts.Token);
        using var reader = new StreamReader(stream);
        List<PeopleResponse> peoples = JsonSerializer.Deserialize<List<PeopleResponse>>(await reader.ReadToEndAsync(linkedCts.Token))!;

        var person = peoples.Single(x => x.imie.Equals(category.imie, StringComparison.OrdinalIgnoreCase) && x.nazwisko.Equals(category.nazwisko, StringComparison.OrdinalIgnoreCase));
        var type = person.GetType();
        var property = type.GetProperty(category.question);
        var answer = await service.CompletationsAsync(question!.GetValue<string>()!, property?.GetValue(person)?.ToString() ?? "MISSING CONTEXT", linkedCts.Token);
        var response = await client.SendAnswerAsync(token, answer, linkedCts.Token);
        return Results.Ok(response);
    }

    internal static async Task<IResult> KnowledgeTaskAsync(IOpenAISemanticKernalService service, ITasksAiDevsClient client, CancellationToken cancellationToken)
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(120));
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, cts.Token);

        var token = await client.GetTokenAsync("knowledge", linkedCts.Token);
        JsonObject task = await client.GetTaskAsync(token, null, linkedCts.Token);
        task.TryGetPropertyValue("question", out var question);

        var answer = await service.KnowledgeTaskAsync(question!.GetValue<string>(), linkedCts.Token);
        var response = await client.SendAnswerAsync(token, answer, linkedCts.Token);
        return Results.Ok(response);
    }

    internal static async Task<IResult> ToolsTaskAsync(IOpenAIService service, ITasksAiDevsClient client, CancellationToken cancellationToken)
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(120));
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, cts.Token);

        var token = await client.GetTokenAsync("tools", linkedCts.Token);
        JsonObject task = await client.GetTaskAsync(token, null, linkedCts.Token);
        task.TryGetPropertyValue("question", out var question);

        var systemPromptBuilder = new StringBuilder();
        systemPromptBuilder.Append("Convert youser input to action.");
        systemPromptBuilder.Append("Answer JSON {\"tool\":\"ToDo(action to do in future)|Calendar(action to add event to calendar)\",\"desc\":\"Summarized action description\",\"date\":\"YYYY-MM-DD(date of action only for calendar action)\"}");
        systemPromptBuilder.Append("Today date is" + DateTime.Now.Date.ToString("yyyy-MM-dd"));
        systemPromptBuilder.Append("example###");
        systemPromptBuilder.Append("Przypomnij mi, że mam kupić mleko = {\"tool\":\"ToDo\",\"desc\":\"Kup mleko\" }");
        systemPromptBuilder.Append("Jutro mam spotkanie z Marianem = {\"tool\":\"Calendar\",\"desc\":\"Spotkanie z Marianem\",\"date\":\"2024-04-10\"}");
        systemPromptBuilder.Append("###");

        var action = await service.CompletionsAsync(systemPromptBuilder.ToString(), question!.GetValue<string>(), linkedCts.Token);
        var answer = JsonSerializer.Deserialize<ActionToDo>(action);
        var response = await client.SendAnswerAsync(token, answer, linkedCts.Token);
        return Results.Ok(response);
    }

    internal static async Task<IResult> GnomeTaskAsync(IOpenAIService service, ITasksAiDevsClient client, CancellationToken cancellationToken)
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(120));
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, cts.Token);

        var token = await client.GetTokenAsync("gnome", linkedCts.Token);
        JsonObject task = await client.GetTaskAsync(token, null, linkedCts.Token);
        task.TryGetPropertyValue("url", out var url);

        var systemPromptBuilder = new StringBuilder();
        systemPromptBuilder.Append("Analyze the image and return colour of hat.");
        systemPromptBuilder.Append("Answer should be in Polish.");
        systemPromptBuilder.Append("When there is no hat in the image, return \"error\"");
        var answer = await service.AnalyzeImageAsync(systemPromptBuilder.ToString(), url!.GetValue<string>(), linkedCts.Token);

        var response = await client.SendAnswerAsync(token, answer, linkedCts.Token);
        return Results.Ok(response);
    }

    private static async Task<string> GetHintAsync(ITasksAiDevsClient client, string token, CancellationToken ct)
    {
        JsonObject task = await client.GetTaskAsync(token, null, ct);
        task.TryGetPropertyValue("hint", out var hint);
        return hint!.GetValue<string>();
    }

    private static async Task<string> GuessPersonAsync(
        IOpenAIService service,
        ITasksAiDevsClient client,
        string token,
        string system,
        List<string> hints,
        CancellationToken cancellationToken)
    {
        var answer = await service.CompletionsAsync(
            system,
            $"Guess who I'm thinking of, here are the hints:\n{string.Join("\n-", hints)}",
            cancellationToken);

        if (answer.Equals("NEED MORE HINTS", StringComparison.OrdinalIgnoreCase))
        {
            hints.Add(await GetHintAsync(client, token, cancellationToken));
            return await GuessPersonAsync(service, client, token, system, hints, cancellationToken);
        }
        else
        {
            return answer;
        }
    }
}
public record PersonCategory(string imie, string nazwisko, string question);
public record ActionToDo(string tool, string desc, string date);
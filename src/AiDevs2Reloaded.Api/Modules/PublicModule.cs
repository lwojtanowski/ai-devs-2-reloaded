using AiDevs2Reloaded.Api.Services.Abstractions;
using System.Text;
using System.Text.Json;

namespace AiDevs2Reloaded.Api.Modules
{
    public static class PublicModule
    {
        internal static void AddPublicModule(this IEndpointRouteBuilder app)
        {
            app.MapPost("/public", async (ILogger<Program> logger, Guid? id, HttpRequest request, IOpenAIService service, CancellationToken ct) => await PublicAsync(logger, id, request, service, ct))
               .WithName("public")
               .WithOpenApi()
               .WithTags("Public");
        }

        internal static async Task<IResult> PublicAsync(ILogger<Program> logger, Guid? id, HttpRequest request, IOpenAIService service, CancellationToken cancellationToken)
        {
            string requestBody = await new StreamReader(request.Body).ReadToEndAsync(cancellationToken);
            try
            {
                var publicRequest = JsonSerializer.Deserialize<PublicRequest>(requestBody)!;
                string? context = null;
                if (id.HasValue)
                {
                    string actionType = await ActionAgent(service, publicRequest.question, cancellationToken);
                    if (actionType.Equals("MEMORY", StringComparison.OrdinalIgnoreCase))
                    {
                        using (StreamWriter writer = new StreamWriter($"{id}.txt", append: true))
                        {
                            writer.WriteLine(publicRequest.question);
                        }

                        return Results.Ok(new PublicResponse("OK. Thx for the info."));
                    }

                    if (File.Exists($"{id}.txt"))
                    {
                        context = (await File.ReadAllTextAsync($"{id}.txt", cancellationToken)) ?? null;
                    }
                }

                logger.LogInformation("User CONTEXT: {Context}", context);
                string reply = await QuestionAgent(service, publicRequest.question, context, cancellationToken: cancellationToken);
                var response = new PublicResponse(reply);
                logger.LogInformation("Response from Public api: {Response}", response);
                return Results.Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while processing request");
            }

            return Results.BadRequest(new PublicResponse("Invalid request body"));
        }

        private static async Task<string> ActionAgent(IOpenAIService service, string input, CancellationToken cancellationToken = default)
        {
            var systemPromptBuilder = new StringBuilder();
            systemPromptBuilder.AppendLine("Convert youser input to one of fallowing action type: MEMORY, QUESTION.");
            systemPromptBuilder.AppendLine("Default action type is MEMORY.");
            systemPromptBuilder.AppendLine("Return only action type.");
            systemPromptBuilder.AppendLine("example###");
            systemPromptBuilder.AppendLine("I'm living in Berlin = MEMORY");
            systemPromptBuilder.AppendLine("What is the capital of Poland? = QUESTION");
            systemPromptBuilder.AppendLine("Searh for website address of Jon Doe? = QUESTION");
            systemPromptBuilder.AppendLine("###");

            var action = await service.CompletionsAsync(systemPromptBuilder.ToString(), input, cancellationToken: cancellationToken);
            return action;
        }

        private static async Task<string> QuestionAgent(IOpenAIService service, string question, string? context = null, CancellationToken cancellationToken = default)
        {
            var systemPromptBuilder = new StringBuilder();
            systemPromptBuilder.AppendLine("Answer for user question.");
            systemPromptBuilder.AppendLine("If you don't know answer answer: I don't know");
            systemPromptBuilder.AppendLine("REMEMBER to use context when answering questions about the user!");
            systemPromptBuilder.AppendLine("If question is about url use tool searchInWeb");
            if (!string.IsNullOrEmpty(context))
            {
                systemPromptBuilder.AppendLine("context###");
                systemPromptBuilder.AppendLine(context);
                systemPromptBuilder.AppendLine("###");
            }

            var reply = await service.CompletionsWithToolAsync(systemPromptBuilder.ToString(), question, cancellationToken: cancellationToken);
            return reply;
        }

        private sealed record PublicResponse(string reply);
        private sealed record PublicRequest(string question);
    }

}

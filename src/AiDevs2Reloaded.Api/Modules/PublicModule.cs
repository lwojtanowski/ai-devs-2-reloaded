using AiDevs2Reloaded.Api.Services.Abstractions;
using System.Text;
using System.Text.Json;

namespace AiDevs2Reloaded.Api.Modules
{
    public static class PublicModule
    {
        internal static void AddPublicModule(this IEndpointRouteBuilder app)
        {
            app.MapPost("/public", async (HttpRequest request, IOpenAIService service, CancellationToken ct) => await PublicAsync(request, service, ct))
               .WithName("public")
               .WithOpenApi()
               .WithTags("Public");
        }

        internal static async Task<IResult> PublicAsync(HttpRequest request, IOpenAIService service, CancellationToken cancellationToken)
        {
            string requestBody = await new StreamReader(request.Body).ReadToEndAsync(cancellationToken);
            try
            {
                var publicRequest = JsonSerializer.Deserialize<PublicRequest>(requestBody)!;
                var systemPromptBuilder = new StringBuilder();
                systemPromptBuilder.AppendLine("Answer for user question.");
                systemPromptBuilder.AppendLine("If you don't know answer answer: I don't know");

                var reply = await service.CompletionsAsync(systemPromptBuilder.ToString(), publicRequest.question, cancellationToken);

                return Results.Ok(new PublicResponse(reply));
            }
            catch
            {
                return Results.BadRequest(new PublicResponse("Invalid request body"));
            }
        }

        private sealed record PublicResponse(string reply);
        private sealed record PublicRequest(string question);
    }

}

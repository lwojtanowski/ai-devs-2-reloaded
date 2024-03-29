using AiDevs2Reloaded.Api.Services.Abstractions;

namespace AiDevs2Reloaded.Api.Modules;

internal static class SampleModule
{
    internal static void AddSampleModule(this IEndpointRouteBuilder app)
    {
        app.MapGet("/function-calling", async (IOpenAIService service, CancellationToken ct) => await FunctionCallingSampleAsync(service, ct))
           .WithName("function-calling")
           .WithOpenApi()
           .WithTags("Samples");
    }

    internal static async Task<IResult> FunctionCallingSampleAsync(IOpenAIService service, CancellationToken cancellationToken)
    {
        var response = await service.AddUserAsync("Dodaj mnie do systemu: Łukasz Wojtanowski urodzony w 1980", cancellationToken);
        return Results.Ok(response);
    }
}

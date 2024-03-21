namespace AiDevs2Reloaded.Api.Contracts.OpenAI;

public sealed record ModerationResponse(string Id, string Model, List<ModerationResult> Results)
{
}
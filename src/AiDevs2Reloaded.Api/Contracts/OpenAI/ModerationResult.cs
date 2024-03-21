namespace AiDevs2Reloaded.Api.Contracts.OpenAI;

public sealed record ModerationResult(bool Flagged, ModerationCategory Categories, ModerationCategoryScores CategoryScores)
{
}

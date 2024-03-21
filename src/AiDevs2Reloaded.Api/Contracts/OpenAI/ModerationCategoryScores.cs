namespace AiDevs2Reloaded.Api.Contracts.OpenAI;

public sealed record ModerationCategoryScores(
    double Sexual,
    double Hate, 
    double Harassment,
    double SelfHarm,
    double SexualMinors,
    double HateThreatening,
    double ViolenceGraphic, 
    double SelfHarmIntent, 
    double SelfHarmInstructions, 
    double HarassmentThreatening, 
    double Violence)
{
}


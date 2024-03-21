namespace AiDevs2Reloaded.Api.Contracts.OpenAI;

public sealed record ModerationCategory(
    bool Sexual, 
    bool Hate, 
    bool Harassment, 
    bool SelfHarm, 
    bool SexualMinors,
    bool HateThreatening, 
    bool ViolenceGraphic, 
    bool SelfHarmIntent, 
    bool SelfHarmInstructions, 
    bool HarassmentThreatening, 
    bool Violence)
{
}


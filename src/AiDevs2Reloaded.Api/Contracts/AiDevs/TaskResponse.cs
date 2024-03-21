namespace AiDevs2Reloaded.Api.Contracts.AIDevs;

public sealed record TaskResponse(
    int Code, 
    string Msg, 
    string? Cookie, 
    List<string>? Input,
    List<string>? Blog)
{
}

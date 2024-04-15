namespace AiDevs2Reloaded.Api.Contracts.RenderForm;

public sealed record RenderImageRequest(string template, Dictionary<string, string> data)
{
}

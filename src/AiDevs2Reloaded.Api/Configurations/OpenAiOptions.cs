namespace AiDevs2Reloaded.Api.Configurations;

public class OpenAiOptions
{
    public const string OpenAI = "OpenAI";

    public string BaseUrl { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
}

using AiDevs2Reloaded.Api.Configurations;
using AiDevs2Reloaded.Api.Exceptions;
using AiDevs2Reloaded.Api.Services.Abstractions;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace AiDevs2Reloaded.Api.Services;

public class OpenAIServices : IOpenAIService
{
    private readonly OpenAiOptions _options;
    private readonly ILogger<OpenAIServices> _logger;
    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public OpenAIServices(IOptions<OpenAiOptions> options, ILogger<OpenAIServices> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task<List<string>> GenerateBlogPostAsync(string input, CancellationToken cancellationToken = default)
    {
        var client = new OpenAIClient(_options.ApiKey);
        var systemPromptBuilder = new StringBuilder();
        systemPromptBuilder.AppendLine("You are a grate blog writter for any topic");
        systemPromptBuilder.AppendLine("Your task is to write a blog post about topic and provided chapters by user and ONLY for that");
        systemPromptBuilder.AppendLine("Requirments for blog post:");
        systemPromptBuilder.AppendLine("-return in POLISH language");
        systemPromptBuilder.AppendLine("-return result that contains one field \"chapters\" that is an array in JSON format ");
        systemPromptBuilder.AppendLine("-return only chapter text without chapter name/chapter title");
        systemPromptBuilder.AppendLine("example ###");
        systemPromptBuilder.AppendLine("{\"chapters\": [\"tekst\", \"tekst\", \"tekst\", \"tekst\"] }");
        systemPromptBuilder.AppendLine("###");

        List<ChatRequestMessage> messages = new()
        {
             new ChatRequestSystemMessage(systemPromptBuilder.ToString()),
             new ChatRequestUserMessage(input)
        };

        var chatCompletionsOptions = new ChatCompletionsOptions("gpt-4", messages);

        try
        {
            var response = await client.GetChatCompletionsAsync(chatCompletionsOptions, cancellationToken);
            if (response.HasValue)
            {
                var message = response.Value.Choices
                    .Select(x => x.Message)
                    .Where(m => m.Role == ChatRole.Assistant)
                    .Select(m => m.Content)
                    .FirstOrDefault();

                var bloggerRespinse = JsonSerializer.Deserialize<BloggerResponse>(message!, _jsonSerializerOptions)!;

                return bloggerRespinse.Chapters;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while retriving response from OpenAI");
            throw;
        }

        throw new BlogPostGenerationException();
    }
}

public sealed record BloggerResponse(List<string> Chapters);
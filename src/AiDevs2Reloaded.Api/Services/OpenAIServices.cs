using AiDevs2Reloaded.Api.Configurations;
using AiDevs2Reloaded.Api.Exceptions;
using AiDevs2Reloaded.Api.Services.Abstractions;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Options;
using PuppeteerSharp;
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

    public async Task<string> CompletationsAsync(string input, string context, CancellationToken cancellationToken = default)
    {
        var client = new OpenAIClient(_options.ApiKey);
        var systemPromptBuilder = new StringBuilder();
        systemPromptBuilder.AppendLine("Use only information from context");
        systemPromptBuilder.AppendLine("context ###");
        systemPromptBuilder.AppendLine(context);
        systemPromptBuilder.AppendLine("###");

        List<ChatRequestMessage> messages = new()
        {
             new ChatRequestSystemMessage(systemPromptBuilder.ToString()),
             new ChatRequestUserMessage(input)
        };

        var chatCompletionsOptions = new ChatCompletionsOptions("gpt-3.5-turbo", messages);

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

                return message!;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while retriving response from OpenAI");
            throw;
        }

        throw new BlogPostGenerationException();
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

    public async Task<string> VerifyAsync(string input, CancellationToken cancellationToken = default)
    {
        var client = new OpenAIClient(_options.ApiKey);
        var systemPromptBuilder = new StringBuilder();
        systemPromptBuilder.AppendLine("You are a lie detector");
        systemPromptBuilder.AppendLine("Your task is to answer if answer for provided question is a lie.");
        systemPromptBuilder.AppendLine("Answer only YES or NO");

        List<ChatRequestMessage> messages = new()
        {
             new ChatRequestSystemMessage(systemPromptBuilder.ToString()),
             new ChatRequestUserMessage(input)
        };

        var chatCompletionsOptions = new ChatCompletionsOptions("gpt-3.5-turbo", messages);

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

                return message!;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while retriving response from OpenAI");
            throw;
        }

        throw new BlogPostGenerationException();
    }

    public async Task<IReadOnlyList<EmbeddingItem>> EmbeddingAsync(List<string> input, CancellationToken cancellationToken = default)
    {
        var client = new OpenAIClient(_options.ApiKey);
        EmbeddingsOptions embeddingsOptions = new()
        {
            DeploymentName = "text-embedding-ada-002",
        };

        input.ForEach(x => embeddingsOptions.Input.Add(x));

        try
        {
            var response = await client.GetEmbeddingsAsync(embeddingsOptions, cancellationToken);
            return response.Value.Data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while retriving response from OpenAI");
            throw;
        }

        throw new MissingEmbeddingException();
    }

    public async Task<string> AudioToSpeechAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        var client = new OpenAIClient(_options.ApiKey);

        var transcriptionOptions = new AudioTranscriptionOptions()
        {
            DeploymentName = "whisper-1",
            AudioData = BinaryData.FromStream(stream),
            ResponseFormat = AudioTranscriptionFormat.Verbose,
        };

        try
        {
            var response = await client.GetAudioTranscriptionAsync(transcriptionOptions, cancellationToken);
            return response.Value.Text;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while retriving response from OpenAI");
            throw;
        }

        throw new MissingTranscriptionException();
    }

    public async Task<string> AddUserAsync(string input, CancellationToken cancellationToken = default)
    {
        var client = new OpenAIClient(_options.ApiKey);
        var systemPromptBuilder = new StringBuilder();
        systemPromptBuilder.AppendLine("You are a System");
        systemPromptBuilder.AppendLine("If user provaid his data add him to system");
        systemPromptBuilder.AppendLine("User must provide his name, surname and year of bright");
        systemPromptBuilder.AppendLine("Valid JSON {\"name\":\"name(first name of user\",\"surname\":\"surname(last name of user\",\"year\":1980(user year of bright)}");

        List<ChatRequestMessage> messages = new()
        {
             new ChatRequestSystemMessage(systemPromptBuilder.ToString()),
             new ChatRequestUserMessage(input)
        };

        var chatCompletionsOptions = new ChatCompletionsOptions("gpt-3.5-turbo-0613", messages);
        var addUserFunction = GetAddUserFunctionDefinition();
        chatCompletionsOptions.Functions.Add(addUserFunction);

        var response = await client.GetChatCompletionsAsync(chatCompletionsOptions, cancellationToken);

        if (response.HasValue)
        {
            var choice = response.Value.Choices[0];
            if (choice.FinishReason == CompletionsFinishReason.FunctionCall)
            {
                messages.Add(new ChatRequestAssistantMessage(choice.Message.Content)
                {
                    FunctionCall = choice.Message.FunctionCall,
                });

                if (choice.Message.FunctionCall.Name.Equals(addUserFunction.Name, StringComparison.OrdinalIgnoreCase))
                {
                    string unvalidatedArguments = choice.Message.FunctionCall.Arguments;
                    var addUserRequest = JsonSerializer.Deserialize<AddUserRequest>(unvalidatedArguments, _jsonSerializerOptions)!;

                    var functionResultData = AddUserFunctionResultData(addUserRequest);
                    var functionResponseMessage = new ChatRequestFunctionMessage(
                        name: choice.Message.FunctionCall.Name,
                        content: JsonSerializer.Serialize(
                            functionResultData,
                            new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));

                    messages.Add(functionResponseMessage);

                    var successChatCompletionsOptions = new ChatCompletionsOptions("gpt-3.5-turbo-0613", messages);
                    var successResponse = await client.GetChatCompletionsAsync(successChatCompletionsOptions, cancellationToken);
                    return successResponse.Value.Choices[0].Message.Content;
                }
            }
        }

        throw new NotImplementedException();
    }

    public async Task<string> CompletionsAsync(string system, string input, bool jsonObject = false, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("User input: {Input}", input);
        var client = new OpenAIClient(_options.ApiKey);

        List<ChatRequestMessage> messages = new()
        {
             new ChatRequestSystemMessage(system),
             new ChatRequestUserMessage(input)
        };

        var chatCompletionsOptions = new ChatCompletionsOptions("gpt-3.5-turbo", messages);
        if (jsonObject)
        {
            chatCompletionsOptions.ResponseFormat = ChatCompletionsResponseFormat.JsonObject;
        }

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

                _logger.LogInformation("OpenAI response: {Response}", message);

                return message!;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while retriving response from OpenAI");
            throw;
        }

        throw new BlogPostGenerationException();
    }

    public async Task<string> CompletionsWithToolAsync(string system, string input, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("User input: {Input}", input);
        var client = new OpenAIClient(_options.ApiKey);

        List<ChatRequestMessage> messages = new()
        {
             new ChatRequestSystemMessage(system),
             new ChatRequestUserMessage(input)
        };

        var chatCompletionsOptions = new ChatCompletionsOptions("gpt-3.5-turbo", messages);
        var searchInWebTool = SearchInWeb();
        chatCompletionsOptions.Functions.Add(searchInWebTool);

        try
        {
            var response = await client.GetChatCompletionsAsync(chatCompletionsOptions, cancellationToken);
            if (response.HasValue)
            {

                if (response.Value.Choices[0]?.FinishReason == CompletionsFinishReason.FunctionCall)
                {
                    var choice = response.Value.Choices[0];
                    if (choice.Message.FunctionCall.Name.Equals(searchInWebTool.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        string unvalidatedArguments = choice.Message.FunctionCall.Arguments;
                        var searchRequest = JsonSerializer.Deserialize<SearchRequest>(unvalidatedArguments, _jsonSerializerOptions)!;
                        var functionResultData = await WebBrowserScrapeAsync(searchRequest.SearchPhrase);
                        _logger.LogInformation("Tool call response: {Response}", functionResultData);
                        return functionResultData;
                    }
                }
                else
                {
                    var message = response.Value.Choices
                        .Select(x => x.Message)
                        .Where(m => m.Role == ChatRole.Assistant)
                        .Select(m => m.Content)
                        .FirstOrDefault();

                    _logger.LogInformation("OpenAI response: {Response}", message);

                    return message!;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while retriving response from OpenAI");
            throw;
        }

        throw new BlogPostGenerationException();
    }

    public async Task<string> AnalyzeImageAsync(string system, string url, CancellationToken cancellationToken = default)
    {
        var client = new OpenAIClient(_options.ApiKey);

        List<ChatRequestMessage> messages = new()
        {
             new ChatRequestSystemMessage(system),
             new ChatRequestUserMessage(new ChatMessageImageContentItem(new Uri(url), ChatMessageImageDetailLevel.Low))
        };

        var chatCompletionsOptions = new ChatCompletionsOptions("gpt-4-turbo", messages);

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

                return message!;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while retriving response from OpenAI");
            throw;
        }

        throw new BlogPostGenerationException();
    }

    private FunctionDefinition GetAddUserFunctionDefinition()
    {
        var function = new FunctionDefinition
        {
            Name = "addUser",
            Description = "Add user to the system",
            Parameters = BinaryData.FromObjectAsJson(
                new
                {
                    Type = "object",
                    Properties = new
                    {
                        Name = new
                        {
                            Type = "string",
                            Description = "provide first name of the user",
                        },
                        Surname = new
                        {
                            Type = "string",
                            Description = "provide last name of the user",
                        },
                        Year = new
                        {
                            Type = "integer",
                            Description = "provide year of birth of the user",
                        }
                    },
                    Required = new[] { "Name", "Surname", "Year" }
                },
                new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase })
        };

        return function;
    }

    private FunctionDefinition SearchInWeb()
    {
        var function = new FunctionDefinition
        {
            Name = "searchUrlInWeb",
            Description = "Search url in web browser",
            Parameters = BinaryData.FromObjectAsJson(
                new
                {
                    Type = "object",
                    Properties = new
                    {
                        SearchPhrase = new
                        {
                            Type = "string",
                            Description = "Paraphrased text optimal for search engines"
                        }
                    },
                    Required = new[] { "SearchPhrase" }
                },
                new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase })
        };

        return function;
    }

    private string AddUserFunctionResultData(AddUserRequest request)
    {
        var message = $"User {request.Name} {request.Surname} born in {request.Year} has been added to the system";
        _logger.LogInformation(message);
        return message;
    }

    private async Task<string> WebBrowserScrapeAsync(string searchPhrase)
    {
        using var browserFetcher = new BrowserFetcher();
        await browserFetcher.DownloadAsync();
        await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
        await using var page = await browser.NewPageAsync();
        await page.GoToAsync($"https://duckduckgo.com/?t=h_&q={searchPhrase.Replace(" ", "+")}&ia=web");
        //await page.WaitForExpressionAsync("document.querySelector('[data-testid=\"about_official-website\"]')!=null");
        await page.WaitForExpressionAsync("document.querySelector('[data-testid=\"result-extras-url-link\"]')!=null");
        var val = await page.EvaluateFunctionAsync<string>("()=>document.querySelector('[data-testid=\"about_official-website\"]')?.href ?? null");
        var val2 = await page.EvaluateFunctionAsync<string>("()=>document.querySelector('[data-testid=\"result-extras-url-link\"]').href");
        return val ?? val2;
    }
}

public sealed record BloggerResponse(List<string> Chapters);
public sealed record AddUserRequest(string Name, string Surname, int Year);
public sealed record SearchRequest(string SearchPhrase);
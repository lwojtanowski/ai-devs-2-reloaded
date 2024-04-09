using AiDevs2Reloaded.Api.Configurations;
using AiDevs2Reloaded.Api.Plugins;
using AiDevs2Reloaded.Api.Services.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Refit;
using System.Globalization;
using System.Text;

namespace AiDevs2Reloaded.Api.Services;
public class OpenAISemanticKernalService : IOpenAISemanticKernalService
{
    private readonly ILogger<OpenAISemanticKernalService> _logger;
    private readonly Kernel _kernel;

    public OpenAISemanticKernalService(IOptions<OpenAiOptions> options, ILogger<OpenAISemanticKernalService> logger)
    {
        _logger = logger;

        var builder = Kernel.CreateBuilder();
        builder.AddOpenAIChatCompletion(
            "gpt-4-turbo",
            options.Value.ApiKey);

        builder.Plugins.Services.AddRefitClient<ICountryApi>()
            .ConfigureHttpClient(client => client.BaseAddress = new Uri("https://restcountries.com"));

        builder.Plugins.Services.AddRefitClient<INbpApi>()
            .ConfigureHttpClient(client =>
            {
                client.BaseAddress = new Uri("https://api.nbp.pl");
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

        builder.Plugins.AddFromType<NbpApiPlugin>();
        builder.Plugins.AddFromType<CountryApiPlugin>();

        _kernel = builder.Build();
        _kernel.Culture = CultureInfo.GetCultureInfo("pl-PL");
    }

    public async Task<string> KnowledgeTaskAsync(string question, CancellationToken cancellationToken = default)
    {
        var history = new ChatHistory();

        var systemPrompt = new StringBuilder();
        systemPrompt.AppendLine("Keep numbers without formating. ");
        systemPrompt.AppendLine("examples ###");
        systemPrompt.AppendLine("85,963,741=85963741 ");
        systemPrompt.AppendLine("8 963 741=8963741 ");
        systemPrompt.AppendLine("###");

        history.AddSystemMessage(systemPrompt.ToString());
        history.AddUserMessage(question);

        var chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();

        OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
        {
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
        };

        var result = await chatCompletionService.GetChatMessageContentAsync(
            history,
            executionSettings: openAIPromptExecutionSettings,
            kernel: _kernel, 
            cancellationToken: cancellationToken);

        _logger.LogInformation("Assistant chat result: {Result}", result.Content);

        return result.Content ?? string.Empty;
    }
}
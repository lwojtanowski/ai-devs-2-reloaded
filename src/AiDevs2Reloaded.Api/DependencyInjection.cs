using AiDevs2Reloaded.Api.Configurations;
using AiDevs2Reloaded.Api.HttpClients;
using AiDevs2Reloaded.Api.HttpClients.Abstractions;
using AiDevs2Reloaded.Api.Services;
using AiDevs2Reloaded.Api.Services.Abstractions;
using Microsoft.Extensions.Options;

namespace AiDevs2Reloaded.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AiDevs2ReloadedOptions>(configuration.GetSection(AiDevs2ReloadedOptions.AiDevs2Reloaded));
        services.Configure<OpenAiOptions>(configuration.GetSection(OpenAiOptions.OpenAI));

        services.AddHttpClient<ITasksAiDevsClient, TasksAiDevsClient>((services, client) =>
        {
            var options = services.GetRequiredService<IOptions<AiDevs2ReloadedOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl);
        });

        services.AddHttpClient<IOpenAIModerationClient, OpenAIModerationClient>((services, client) =>
        {
            var options = services.GetRequiredService<IOptions<OpenAiOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl);
        });

        services.AddScoped<IOpenAIService, OpenAIServices>();

        return services;
    }
}

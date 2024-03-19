using AiDevs2Reloaded.Api.Configurations;
using AiDevs2Reloaded.Api.HttpClients;
using AiDevs2Reloaded.Api.HttpClients.Abstractions;
using Microsoft.Extensions.Options;

namespace AiDevs2Reloaded.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AiDevs2ReloadedOptions>(configuration.GetSection(AiDevs2ReloadedOptions.AiDevs2Reloaded));
        services.Configure<OpenAiOptions>(configuration.GetSection(OpenAiOptions.OpenAi));

        services.AddHttpClient<ITasksAiDevsClient, TasksAiDevsClient>((services, client) =>
        {
            var options = services.GetRequiredService<IOptions<AiDevs2ReloadedOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl);
        });

        return services;
    }
}

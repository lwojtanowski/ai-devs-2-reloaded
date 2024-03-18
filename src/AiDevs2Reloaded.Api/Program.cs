using AiDevs2Reloaded.Api.Configurations;
using AiDevs2Reloaded.Api.HttpClients;
using AiDevs2Reloaded.Api.HttpClients.Abstractions;
using AiDevs2Reloaded.Api.TaskModule;
using Microsoft.Extensions.Options;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((hostingContext, loggerConfiguration) =>
    loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration)
       .Enrich.FromLogContext()
          .WriteTo.Console());

builder.Services.Configure<AiDevs2ReloadedOptions>(
    builder.Configuration.GetSection(AiDevs2ReloadedOptions.AiDevs2Reloaded));

builder.Services.Configure<OpenAiOptions>(
    builder.Configuration.GetSection(OpenAiOptions.OpenAi));

builder.Services.AddHttpClient<ITasksAiDevsClient, TasksAiDevsClient>((services, client) =>
{
    var options = services.GetRequiredService<IOptions<AiDevs2ReloadedOptions>>().Value;
    client.BaseAddress = new Uri(options.BaseUrl);
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseSerilogRequestLogging();

app.AddTaskModule();

app.Run();

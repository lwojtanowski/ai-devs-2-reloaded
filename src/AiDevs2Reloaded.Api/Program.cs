using AiDevs2Reloaded.Api;
using AiDevs2Reloaded.Api.Modules;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
{
    // Add services to the container.
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services
        .AddPresentation(builder.Configuration)
        .AddEndpointsApiExplorer()
        .AddSwaggerGen();

    builder.Host.UseSerilog((hostingContext, loggerConfiguration) =>
        loggerConfiguration
            .ReadFrom.Configuration(hostingContext.Configuration)
            .Enrich.FromLogContext()
            .WriteTo.Console());
}

var app = builder.Build();
{
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseSerilogRequestLogging();
    app.AddTaskModule();
    app.AddSampleModule();
    app.AddPublicModule();
    app.Run();
}

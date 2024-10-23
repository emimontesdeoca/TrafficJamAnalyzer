using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using TrafficJamAnalyzer.Shared.Clients;
using TrafficJamAnalyzer.Workers.Analyzer;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();
builder.Services.AddProblemDetails();

builder.Services.AddHttpClient<AiApiClient>(client =>
{
    client.BaseAddress = new("https+http://aiservice");
});

builder.Services.AddHttpClient<WebApiClient>(client =>
{
    client.BaseAddress = new("https+http://apiservice");
});

builder.Services.AddHttpClient<ScrapApiClient>(client =>
{
    client.BaseAddress = new("https+http://scrapservice");
});

builder.Services.AddSingleton<WorkerService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

app.MapDefaultEndpoints();

// Map the endpoint with logging
app.MapGet("/health", (WorkerService worker) =>
{
    return Results.Ok(worker.LastRun);
});

using (var scope = app.Services.CreateScope())
{
    var worker = scope.ServiceProvider.GetRequiredService<WorkerService>();
    worker.Start();
}

app.Run();
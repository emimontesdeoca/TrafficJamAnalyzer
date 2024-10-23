using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TrafficJamAnalyzer.Shared.Models;

// Builder
var builder = WebApplication.CreateBuilder(args);
var kernelBuilder = Kernel.CreateBuilder();

var deploymentName = builder.Configuration["OpenAI:DeploymentName"];
var endpoint = builder.Configuration["OpenAI:Endpoint"];
var apiKey = builder.Configuration["OpenAI:ApiKey"];
var prompt = builder.Configuration["OpenAI:Prompt"];

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

// Add OpenAI
kernelBuilder.AddAzureOpenAIChatCompletion(deploymentName!, endpoint!, apiKey!);

var app = builder.Build();
var logger = app.Logger;
var kernel = kernelBuilder.Build();

// build chat
var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

// Map the endpoint with logging
app.MapGet("/analyze/{identifier}", async (string identifier, ILogger<Program> logger) =>
{
    logger.LogInformation("Received analyze request with identifier: {Identifier}", identifier);

    var history = new ChatHistory();
    history.AddSystemMessage("You are a useful assistant that replies using a direct style");

    var imageUrl = $"http://cic.tenerife.es/e-Traffic3/data/{identifier}.jpg";


    // "Prompt": "The image I'm going to provide you is an image from a CCTV that shows a road, I need you to give me a JSON object with 'Title' which is title in the top left and 'Traffic' which is an integer from 0 to 100 which shows the amout of traffic jam and the 'Date' that is on the bottom right, please only provide the JSON result and nothing else. Return only the json object without any markdown. If you a lot of lanes, please focus on the one that is busy when checking for the traffic, so, if you see 4 lanes and only 2 are full, it means that the traffic is jammed."

    var collectionItems = new ChatMessageContentItemCollection
    {
        new TextContent(prompt),
        new ImageContent { Uri = new Uri(imageUrl) },
    };

    history.AddUserMessage(collectionItems);

    logger.LogInformation("Image URL generated: {ImageUrl}", imageUrl);
    logger.LogInformation("Chat history created: {ChatHistory}", JsonConvert.SerializeObject(history));

    var result = await chatCompletionService.GetChatMessageContentsAsync(history);

    var content = result[^1].Content;

    if (content == null)
    {
        logger.LogWarning("No content received from chatCompletionService.");
        return new TrafficJamAnalyzeResult();
    }

    logger.LogInformation("Content received: {Content}", content);

    var analyzeResult = new TrafficJamAnalyzeResult
    {
        CreatedAt = DateTime.UtcNow,
        Result = JsonConvert.DeserializeObject<TrafficJamAnalyze>(content)!,
        SourceUrl = imageUrl
    };

    logger.LogInformation("Analysis result created: {AnalyzeResult}", JsonConvert.SerializeObject(analyzeResult));

    return analyzeResult;
});

logger.LogInformation("Application starting up.");
app.Run();
logger.LogInformation("Application shut down.");
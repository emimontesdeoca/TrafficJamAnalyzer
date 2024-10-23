
var builder = DistributedApplication.CreateBuilder(args);


var sqldb = builder.AddSqlServer("sql")
    .WithDataVolume()
    .AddDatabase("sqldb");

var apiService = builder.AddProject<Projects.TrafficJamAnalyzer_Microservices_WebApiService>("apiservice")
    .WithReference(sqldb);

var aiService = builder.AddProject<Projects.TrafficJamAnalyzer_Microservices_AiApiService>("aiservice");

var scrapService = builder.AddProject<Projects.TrafficJamAnalyzer_Microservices_ScraperApiService>("scrapservice");

var worker = builder.AddProject<Projects.TrafficJamAnalyzer_Workers_Analyzer>("worker")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WithReference(aiService)
    .WithReference(scrapService);


builder.AddProject<Projects.TrafficJamAnalyzer_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WithReference(scrapService);

builder.Build().Run();
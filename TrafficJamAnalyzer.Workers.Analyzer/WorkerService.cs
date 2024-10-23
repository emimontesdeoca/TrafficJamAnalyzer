using TrafficJamAnalyzer.Shared.Clients;
using TrafficJamAnalyzer.Shared.Models;

namespace TrafficJamAnalyzer.Workers.Analyzer
{
    public class WorkerService
    {
        private readonly WebApiClient _webApiClient;
        private readonly AiApiClient _aiApiClient;
        private readonly ILogger<BackgroundService> _logger;

        public DateTime LastRun;

        public WorkerService(
            WebApiClient webApiClient,
            AiApiClient aiApiClient,
            ILogger<BackgroundService> logger)
        {
            _webApiClient = webApiClient;
            _aiApiClient = aiApiClient;
            _logger = logger;
            LastRun = DateTime.UtcNow;
        }

        public void Start()
        {
            Task.Run(async () =>
            {
                // Wait for the database to start correctly
                await Task.Delay(TimeSpan.FromSeconds(10));

                try
                {
                    _logger.LogInformation("Fetching traffic title entries...");

                    var traffics = await _webApiClient.GetTrafficAsync();

                    foreach (var traffic in traffics.Where(x => x.Title.ToLower() == "entry"))
                    {
                        _logger.LogInformation("Processing traffic: {Id}", traffic.Id);

                        var identifier = traffic.Url.Split("/").Last().Replace(".jpg", "");

                        var analyzeResult = await _aiApiClient.AnalyzeAsync(identifier);

                        if (analyzeResult is not null)
                        {
                            traffic.Title = analyzeResult.Result.Title;

                            await _webApiClient.UpdateTrafficAsync(traffic.Id, traffic);

                            _logger.LogInformation("Added title to traffic: {Id}", traffic.Id);
                        }

                        await Task.Delay(TimeSpan.FromSeconds(35));
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred during processing.");
                }

                while (true)
                {
                    try
                    {
                        _logger.LogInformation("Fetching traffic entries...");

                        var traffics = await _webApiClient.GetTrafficAsync();

                        foreach (var traffic in traffics.Where(x => x.Enabled))
                        {
                            _logger.LogInformation("Processing traffic: {Id}", traffic.Id);

                            var identifier = traffic.Url.Split("/").Last().Replace(".jpg", "");

                            var analyzeResult = await _aiApiClient.AnalyzeAsync(identifier);

                            if (analyzeResult is not null)
                            {
                                await _webApiClient.AddTrafficResultAsync(
                                    traffic.Id,
                                    new TrafficResult()
                                    {
                                        CreatedAt = analyzeResult.Result.Date,
                                        TrafficTitle = analyzeResult.Result.Title,
                                        TrafficAmount = analyzeResult.Result.Traffic,
                                    }
                                );
                                _logger.LogInformation("Added analysis result for traffic: {Id}", traffic.Id);
                            }

                            await Task.Delay(TimeSpan.FromSeconds(5));
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "An error occurred during processing.");
                    }

                    _logger.LogInformation("Waiting for the next iteration...");
                    LastRun = DateTime.UtcNow;
                    await Task.Delay(TimeSpan.FromMinutes(1));
                }
            });
        }
    }
}

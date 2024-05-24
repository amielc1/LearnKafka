using Serilog;

namespace KafkaConsumerWikiRest.Services;

public class WorkerService : BackgroundService
{

    private readonly KafkaConsumerService _kafkaConsumer;
    private readonly OpenSearchService _openSearchService;
    private readonly string _indexName;

    public WorkerService(KafkaConsumerService kafkaConsumer, OpenSearchService openSearchService)
    {
        _kafkaConsumer = kafkaConsumer;
        _openSearchService = openSearchService;
        _indexName = "default-stream-index";
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();
        Log.Information("Worker Service Started");
        try
        {
            await _kafkaConsumer.ConsumeMessagesAsync(stoppingToken, message =>
            {
                _openSearchService.Index(message, _indexName);
            });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Amiel");
        }
    }
}
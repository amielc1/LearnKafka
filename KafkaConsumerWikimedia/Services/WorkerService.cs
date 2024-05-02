using Microsoft.Extensions.Hosting;
using Serilog;

namespace KafkaConsumerWikimedia.Services;

public class WorkerService : BackgroundService
{

    private readonly KafkaConsumerService _kafkaConsumerService;

    public WorkerService(
        KafkaConsumerService kafkaConsumerService)
    {
        _kafkaConsumerService = kafkaConsumerService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Log.Information("KafkaConsumerWikimedia WorkerService Started");
        await _kafkaConsumerService.ConsumeAsync(stoppingToken);
    }
}
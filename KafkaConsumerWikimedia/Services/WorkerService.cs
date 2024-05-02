using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KafkaConsumerWikimedia.Services;

public class WorkerService : BackgroundService
{

    private readonly KafkaConsumerService _kafkaConsumerService;
    private readonly ILogger<WorkerService> _logger;

    public WorkerService(
        KafkaConsumerService kafkaConsumerService,
        ILogger<WorkerService> logger)
    {
        _kafkaConsumerService = kafkaConsumerService;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _kafkaConsumerService.ConsumeAsync(stoppingToken);
    }
}
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace WikimediaKafkaProducer.Services
{
    public class WorkerService : BackgroundService
    {
        private readonly EventStreamService _eventStreamService;
        private readonly KafkaProducerService _kafkaProducerService;
        private readonly ILogger<WorkerService> _logger;

        public WorkerService(EventStreamService eventStreamService,
            KafkaProducerService kafkaProducerService,
            ILogger<WorkerService> logger)
        {
            _eventStreamService = eventStreamService;
            _kafkaProducerService = kafkaProducerService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Starting to listen to Wikimedia recent changes...");

            await foreach (var line in _eventStreamService.GetEventsAsync())
            {
                if (stoppingToken.IsCancellationRequested) break;

                _logger.LogInformation($"Producing record: {line}");
                await _kafkaProducerService.ProduceAsync(line);
            }

            _kafkaProducerService.Flush();
        }
    }
}
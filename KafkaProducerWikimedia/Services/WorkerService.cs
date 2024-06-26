﻿using Microsoft.Extensions.Hosting;
using Serilog;

namespace WikimediaKafkaProducer.Services
{
    public class WorkerService : BackgroundService
    {
        private readonly EventStreamService _eventStreamService;
        private readonly KafkaProducerService _kafkaProducerService; 

        public WorkerService(EventStreamService eventStreamService,
            KafkaProducerService kafkaProducerService)
        {
            _eventStreamService = eventStreamService;
            _kafkaProducerService = kafkaProducerService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Log.Information("Starting to listen to Wikimedia recent changes...");

            try
            {
                await foreach (var line in _eventStreamService.GetEventsAsync())
                {
                    if (stoppingToken.IsCancellationRequested) break;

                    Log.Information($"Producing record: {line}");
                    await _kafkaProducerService.ProduceAsync(line);
                }

                _kafkaProducerService.Flush();
            }
            catch (Exception e)
            {
                Log.Error(e, "Failed read from stream");
            }

        }
    }
}
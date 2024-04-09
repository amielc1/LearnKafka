using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace WikimediaKafkaProducer.Services
{
    public class KafkaProducerService
    {
        private readonly IProducer<Null, string> _producer;
        private readonly string _topicName;
        private readonly ILogger<KafkaProducerService> _logger;

        public KafkaProducerService(string bootstrapServers, string topicName, ILogger<KafkaProducerService> logger)
        {
            _producer = new ProducerBuilder<Null, string>(new ProducerConfig { BootstrapServers = bootstrapServers }).Build();
            _topicName = topicName;
            _logger = logger;
        }

        public async Task ProduceAsync(string message)
        {
            try
            {
                var result = await _producer.ProduceAsync(_topicName, new Message<Null, string> { Value = message });
                _logger.LogDebug($"Message produced to topic {_topicName}, partition {result.Partition}, offset {result.Offset}");
            }
            catch (ProduceException<Null, string> e)
            {
                _logger.LogError($"Failed to deliver message to topic {_topicName}: {e.Message} [{e.Error.Code}]");
                throw;
            }
        }

        public void Flush()
        {
            _producer.Flush(TimeSpan.FromSeconds(10));
            _logger.LogInformation("Kafka producer flushed.");
        }
    }
}
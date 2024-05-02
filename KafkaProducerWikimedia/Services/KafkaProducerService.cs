using Confluent.Kafka;
using Serilog;
using System.Text.Json;

namespace WikimediaKafkaProducer.Services
{
    public class KafkaProducerService
    {
        private readonly IProducer<Null, string> _producer;
        private readonly string _topicName; 
        private readonly ProducerConfig _producerConfig;    

        public KafkaProducerService(string bootstrapServers, string topicName )
        {
            _producerConfig = new ProducerConfig
            {
                BootstrapServers = bootstrapServers, 
                //settings for safe producer
                Acks = Acks.All, // ensure ack from all replicas
                MessageSendMaxRetries = int.MaxValue, // retry until timeout reached
                EnableIdempotence = true, // duplicate are not introduced due to network retry

                LingerMs = 20,
                BatchSize = 32*1024,

            };
            _producer = new ProducerBuilder<Null, string>(_producerConfig).Build();
            _topicName = topicName; 
            LogConfiguration(_producerConfig);
        }

        public async Task ProduceAsync(string message)
        {
            try
            { 
                Log.Debug($"Try to  produce to {_producerConfig.BootstrapServers} , topic {_topicName}. {message}");
                var result = await _producer.ProduceAsync(_topicName, new Message<Null, string> { Value = message });
                Log.Debug($"Message produced to topic {_topicName}, partition {result.Partition}, offset {result.Offset}");
            }
            catch (ProduceException<Null, string> e)
            {
                Log.Error($"Failed to deliver message to topic {_topicName}: {e.Message} [{e.Error.Code}]");
                throw;
            }
        }

        public void Flush()
        {
            _producer.Flush(TimeSpan.FromSeconds(10));
            Log.Information("Kafka producer flushed.");
        }
        private void LogConfiguration(ProducerConfig config)
        {
            // Serialize the ProducerConfig object to JSON
            var configJson = JsonSerializer.Serialize(config, new JsonSerializerOptions
            {
                WriteIndented = true, // For better readability in logs
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            });

            Log.Information("ProducerConfig: {@ProducerConfig}",config);
        }
    }
}
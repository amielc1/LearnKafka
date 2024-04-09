using System;
using Confluent.Kafka;

namespace WikimediaKafkaProducer.Services
{
    public class KafkaProducerService
    {
        private readonly IProducer<Null, string> _producer;
        private readonly string _topicName;

        public KafkaProducerService(string bootstrapServers, string topicName)
        {
            var config = new ProducerConfig { BootstrapServers = bootstrapServers };
            _producer = new ProducerBuilder<Null, string>(config).Build();
            _topicName = topicName;
        }

        public async Task ProduceAsync(string message)
        {
            await _producer.ProduceAsync(_topicName, new Message<Null, string> { Value = message });
            Console.WriteLine($"Produced message to topic {_topicName}");
        }

        public void Flush()
        {
            _producer.Flush(TimeSpan.FromSeconds(10));
        }
    }
}
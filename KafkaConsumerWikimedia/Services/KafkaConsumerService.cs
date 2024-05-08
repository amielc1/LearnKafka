using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Serilog;

namespace KafkaConsumerWikimedia.Services;

public class KafkaConsumerService
{
    private readonly IConsumer<Null, string> _consumer;
    private readonly string _topicName;

    public KafkaConsumerService(string bootstrapServers, string topicName, string groupId)
    {
        var config = new ConsumerConfig
        {
            GroupId = groupId,
            BootstrapServers = bootstrapServers,
            AutoOffsetReset = AutoOffsetReset.Latest,
            //read about this 
            EnableAutoCommit = false
        };

        _consumer = new ConsumerBuilder<Null, string>(config).Build();
        _topicName = topicName;
        LogConfiguration(config);
    }

    public async Task ConsumeMessagesAsync(CancellationToken token, Action<string> handleMessage)
    {
        _consumer.Subscribe(_topicName);
        Log.Information("Subscribe to consume from Topic {topic}", _topicName);
        try
        {
            while (!token.IsCancellationRequested)
            {
                var cr = _consumer.Consume(token);

                try
                {
                    handleMessage(cr.Message.Value);
                }
                catch (Exception e)
                {
                    Log.Error(e, "Failed to invoke action while consume message from topic {topic}", _topicName);
                }

                _consumer.Commit(cr);
            }
        }
        catch (OperationCanceledException ex)
        {
            Log.Error(ex, "Failed to consume");
            _consumer.Close();
        }
    }

    private void LogConfiguration(ConsumerConfig config)
    {
        // Serialize the ConsumerConfig object to JSON
        var configJson = JsonSerializer.Serialize(config, new JsonSerializerOptions
        {
            WriteIndented = true, // For better readability in logs
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        });

        Log.Information($"ConsumerConfig: {configJson}");
    }

}
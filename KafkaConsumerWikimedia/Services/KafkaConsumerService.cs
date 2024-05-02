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
            EnableAutoCommit = false
        };

        _consumer = new ConsumerBuilder<Null, string>(config).Build();
        _topicName = topicName;
        LogConfiguration(config);
    }

    public Task ConsumeAsync(CancellationToken token)
    {
        _consumer.Subscribe(_topicName);
        try
        {
            while (true)
            {
                try
                {
                    var cr = _consumer.Consume(token);
                    Log.Debug($"Consumed record with key: {cr.Message.Key} and value: {cr.Message.Value}");
                    // Handle the message, for example, process it or insert into a database

                    // After processing the batch of messages, commit the offsets.
                    _consumer.Commit(cr);
                }
                catch (ConsumeException e)
                {
                    Console.WriteLine($"Error occurred: {e.Error.Reason}");
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Ensure the consumer leaves the group cleanly and final offsets are committed.
            _consumer.Close();
        }
        return Task.CompletedTask;
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
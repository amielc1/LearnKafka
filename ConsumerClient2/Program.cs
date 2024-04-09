using Confluent.Kafka;

var conf = new ConsumerConfig
{
    GroupId = "test-consumer-group2",
    BootstrapServers = "localhost:19092",
    AutoOffsetReset = AutoOffsetReset.Earliest
};

using var c = new ConsumerBuilder<Ignore, string>(conf).Build();
c.Subscribe("AmielTopic");

var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) => {
    e.Cancel = true; // prevent the process from terminating.
    cts.Cancel();
};

try
{
    while (true)
    {
        try
        {
            var cr = c.Consume(cts.Token);
            
            Console.WriteLine(
                $"Consumed message '{cr?.Message.Value}', at: '{cr?.TopicPartitionOffset}'");
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
    c.Close();
}
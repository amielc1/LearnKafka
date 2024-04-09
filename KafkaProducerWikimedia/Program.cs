using WikimediaKafkaProducer.Services;


var httpClient = new HttpClient();
var eventStreamService = new EventStreamService(httpClient);
var kafkaProducerService = new KafkaProducerService("localhost:19092", "wikimedia-recent-changes");

Console.WriteLine("Starting to listen to Wikimedia recent changes...");

await foreach (var line in eventStreamService.GetEventsAsync())
{
    Console.WriteLine($"Producing record: {line}");
    await kafkaProducerService.ProduceAsync(line);
}

kafkaProducerService.Flush();
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using WikimediaKafkaProducer.Services;

namespace WikimediaKafkaProducer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            var eventStreamService = host.Services.GetRequiredService<EventStreamService>();
            var kafkaProducerService = host.Services.GetRequiredService<KafkaProducerService>();

            Console.WriteLine("Starting to listen to Wikimedia recent changes...");

            await foreach (var line in eventStreamService.GetEventsAsync())
            {
                Console.WriteLine($"Producing record: {line}");
                await kafkaProducerService.ProduceAsync(line);
            }

            kafkaProducerService.Flush();
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((_, services) =>
                    services.AddSingleton<HttpClient>()
                        .AddSingleton<EventStreamService>()
                        .AddSingleton<KafkaProducerService>(sp => new KafkaProducerService("localhost:19092", "wikimedia-recent-changes3")));
    }
}
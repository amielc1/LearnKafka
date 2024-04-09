using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using KafkaProducerWikimedia.Config;
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
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.SetBasePath(Directory.GetCurrentDirectory());
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    var configuration = hostContext.Configuration;
                    var kafkaConfig = configuration.GetSection("Kafka").Get<KafkaSettings>();

                    services.AddSingleton<HttpClient>()
                            .AddSingleton<EventStreamService>()
                            .AddSingleton<KafkaProducerService>(sp => new KafkaProducerService(kafkaConfig.BootstrapServers, kafkaConfig.TopicName));
                });
    }


}

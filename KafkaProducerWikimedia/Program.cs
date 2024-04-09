using KafkaProducerWikimedia.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WikimediaKafkaProducer.Services;

namespace WikimediaKafkaProducer
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            await host.RunAsync();
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
                    var kafkaSettings = configuration.GetSection("Kafka").Get<KafkaSettings>();

                    services.Configure<EventStreamSettings>(hostContext.Configuration.GetSection("EventStream"));

                    services
                        .AddSingleton<HttpClient>()
                        .AddSingleton<EventStreamService>()
                        // Register KafkaProducerService with necessary dependencies
                        .AddSingleton<KafkaProducerService>(serviceProvider =>
                        {
                            var logger = serviceProvider.GetRequiredService<ILogger<KafkaProducerService>>();
                            return new KafkaProducerService(kafkaSettings.BootstrapServers, kafkaSettings.TopicName, logger);
                        })
                        .AddHostedService<WorkerService>();
                });
    }
}
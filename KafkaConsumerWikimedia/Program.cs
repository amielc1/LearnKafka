﻿using KafkaConsumerWikimedia.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using KafkaConsumerWikimedia.Services;

namespace KafkaConsumerWikimedia
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.Seq("http://localhost:5341") // Update this URL if your Seq instance is running elsewhere
                .CreateLogger();

            try
            {
                var host = CreateHostBuilder(args).Build();
                await host.RunAsync();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog() // Use Serilog for logging
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.SetBasePath(Directory.GetCurrentDirectory());
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    var configuration = hostContext.Configuration;
                    var kafkaSettings = configuration.GetSection("Kafka").Get<KafkaSettings>();

                    services
                        .AddSingleton<HttpClient>()
                        .AddSingleton<KafkaConsumerService>(serviceProvider =>
                        {
                            var logger = serviceProvider.GetRequiredService<ILogger<KafkaConsumerService>>();
                            return new KafkaConsumerService(kafkaSettings.BootstrapServers, kafkaSettings.TopicName, kafkaSettings.GroupId);
                        })
                        .AddHostedService<WorkerService>();
                });

        // Assume Configuration is a static property or method that retrieves the IConfiguration
        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
    }
}

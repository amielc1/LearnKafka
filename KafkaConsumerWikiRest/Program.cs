using KafkaConsumerWikiRest.Config;
using KafkaConsumerWikiRest.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.Seq("http://localhost:5341"));

builder.Host.ConfigureAppConfiguration((context, config) =>
{
    config.AddEnvironmentVariables();
});

// Load custom settings
var kafkaSettings = builder.Configuration.GetSection("Kafka").Get<KafkaSettings>();
var opensearchSettings = builder.Configuration.GetSection("Opensearch").Get<OpensearchSettings>();

// Add services
builder.Services.AddSingleton(serviceProvider => new OpenSearchService(opensearchSettings.Endpoint));
builder.Services.AddSingleton(serviceProvider => new KafkaConsumerService(kafkaSettings.BootstrapServers, kafkaSettings.TopicName, kafkaSettings.GroupId));
builder.Services.AddHostedService<WorkerService>();
builder.Services.AddHealthChecks(); // Registers health checks services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
//if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();  // This needs to come first

app.UseAuthorization();  // Then use authorization if needed

// Finally map the endpoints
app.UseEndpoints(endpoints =>
{

    endpoints.MapHealthChecks("/health");
    endpoints.MapControllers();
});

app.Run();
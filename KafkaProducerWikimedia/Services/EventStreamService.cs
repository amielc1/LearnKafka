using KafkaProducerWikimedia.Config;
using Microsoft.Extensions.Options;

namespace WikimediaKafkaProducer.Services
{
    public class EventStreamService
    {
        private readonly HttpClient _httpClient;
        private readonly string _streamUrl;

        public EventStreamService(HttpClient httpClient, IOptions<EventStreamSettings> eventStreamSettings)
        {
            _httpClient = httpClient;
            _streamUrl = eventStreamSettings.Value.WikimediaStreamUrl;
        }

        public async IAsyncEnumerable<string> GetEventsAsync()
        {
            using var stream = await _httpClient.GetStreamAsync(_streamUrl);
            using var reader = new StreamReader(stream);
            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                if (!string.IsNullOrEmpty(line) && line.StartsWith("data: {"))
                {
                    yield return line;
                }
            }
        }
    }
}
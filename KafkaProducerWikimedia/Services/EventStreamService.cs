namespace WikimediaKafkaProducer.Services
{
    public class EventStreamService
    {
        private readonly HttpClient _httpClient;
        private const string StreamUrl = "https://stream.wikimedia.org/v2/stream/recentchange";

        public EventStreamService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async IAsyncEnumerable<string> GetEventsAsync()
        { 
            using var stream = await _httpClient.GetStreamAsync(StreamUrl);
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
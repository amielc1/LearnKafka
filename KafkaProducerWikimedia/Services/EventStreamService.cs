using KafkaProducerWikimedia.Config;
using Microsoft.Extensions.Options;
using Serilog;

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

        public string ExtractJson(string input)
        {
            // Find the position of the first opening curly brace
            int startIndex = input.IndexOf('{');
            if (startIndex == -1)
            {
                return "No JSON data found.";
            }

            // Find the position of the last closing curly brace
            int endIndex = input.LastIndexOf('}');
            if (endIndex == -1)
            {
                return "No JSON data found.";
            }

            // Calculate the length of the JSON string
            int length = endIndex - startIndex + 1;

            // Extract the JSON substring
            string jsonData = input.Substring(startIndex, length);
            return jsonData;
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
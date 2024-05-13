using OpenSearch.Client;
using Serilog;

namespace KafkaConsumerWikiRest.Services;

public class Message
{
    public int No { get; set; }
    public string Content { get; set; }
}
public class OpenSearchService
{
    private static int counter = 0;
    private IOpenSearchClient _client;

    public OpenSearchService(string endpoint)
    {
        // Setup connection settings to your OpenSearch cluster
        var settings = new ConnectionSettings(new Uri(endpoint))
            .DefaultIndex("default-index") // Set the default index
            .PrettyJson(true) // Optional: Enable pretty JSON formatting
            .EnableDebugMode(); // Optional: Enable debug mode to see request and response details

        _client = new OpenSearchClient(settings);
        Log.Information("Create Opensearch client with : {endpoint}", endpoint);
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
    public bool Index(string document, string indexName)
    {
        var msg = new Message()
        {
            No = OpenSearchService.counter++,
            Content = ExtractJson(document)
        };

        var response = _client.Index(msg, i => i.Index(indexName));
        Log.Debug("OpenSearchService {index} {IsValid} {@document}", indexName, response.IsValid, msg);
        if (!response.IsValid)
        {
            Log.Error("Failed to index document in {indexName}. Error: {errorInfo}", indexName, response.DebugInformation);
            // Optionally, you can throw an exception or handle the error as needed
        }
        else
        {
            Log.Debug("Document indexed successfully in {indexName}.", indexName);
        }
        return response.IsValid;
    }

    public bool BulkIndex<T>(IEnumerable<T> documents, string indexName) where T : class
    {
        var bulkDescriptor = new BulkDescriptor();
        foreach (var document in documents)
        {
            bulkDescriptor.Index<T>(op => op
                .Document(document)
                .Index(indexName));
        }
        var response = _client.Bulk(bulkDescriptor);
        return response.IsValid;
    }
}
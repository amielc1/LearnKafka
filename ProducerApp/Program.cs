#region 
// Kafka framework dependency in our Application logic
// What if I want to replace Kafka with some other framework??
#endregion
using Confluent.Kafka;

#region 
// Kafka configuration in our Application logic
#endregion
//1 Configure Kafka Producer localhost:19092
var config = new ProducerConfig { BootstrapServers = "localhost:19092" };

#region
// Kafka framework creating producer
#endregion
//2 Build Kafka Producer
using var p = new ProducerBuilder<Null, string>(config).Build();
try
{
    //3 Listen to user input for producing messages
    while (true)
    {
        //4 produce message with Console Input/OutPut
        Console.WriteLine("Produce new message: ");
        var messageInput = Console.ReadLine();

        //5 Validate input, if not valid do not produce message with kafka
        if (string.IsNullOrWhiteSpace(messageInput) is false)
        {
            #region 
            // in the middle of our application logic we have another case of Kafka logic
            #endregion
            //6 produce message with kafka
            var dr = await p.ProduceAsync("amiel_topic", new Message<Null, string> { Value = messageInput });

            #region 
            // log produce message result => SRP
            #endregion
            //7 log message delivery result
            Console.WriteLine($"Delivered '{dr.Value}' to '{dr.TopicPartitionOffset}'");
        }
    }
}
#region 
// Kafka related exception 
#endregion
//8 handle kafka exceptions
catch (ProduceException<Null, string> e)
{
    #region 
    // log error, limited to console logger
    #endregion
    Console.WriteLine($"Delivery failed: {e.Error.Reason}");
}
#region 
// Violation of all S.O.L.I.D principals
// Code is not readable, has many hidden intentions, poor naming 
// Code Tightly couple to the Confluent.Kafka framework 
#endregion
using Azure.Messaging.ServiceBus;
using Azure.Identity;

// name of your Service Bus queue
// the client that owns the connection and can be used to create senders and receivers
ServiceBusClient client;

// the sender used to publish messages to the queue
ServiceBusSender sender;

// number of messages to be sent to the queue
const int numOfMessages = 3;

// The Service Bus client types are safe to cache and use as a singleton for the lifetime
// of the application, which is best practice when messages are being published or read
// regularly.
//
// Set the transport type to AmqpWebSockets so that the ServiceBusClient uses the port 443. 
// If you use the default AmqpTcp, ensure that ports 5671 and 5672 are open.
var clientOptions = new ServiceBusClientOptions
{
    TransportType = ServiceBusTransportType.AmqpWebSockets
};

//TODO: Replace the "<NAMESPACE-NAME>" and "<QUEUE-NAME>" placeholders.
client = new ServiceBusClient(
    "rhyous-learning.servicebus.windows.net",
    new DefaultAzureCredential(),
    clientOptions);
sender = client.CreateSender("myqueue");




try
{
    // create a batch 
    using ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();

    string msg = "";
    ServiceBusMessage sbMessage;
    int i = 0;
    do
    {
        Console.WriteLine("Type in a message to send or type 'exit' to end.");
        msg = Console.ReadLine();
        Console.WriteLine();
        sbMessage = new ServiceBusMessage(msg);
        i++;
        await sender.SendMessageAsync(sbMessage);
    } while (msg != null && !msg.Equals("Exit", StringComparison.OrdinalIgnoreCase));

    Console.WriteLine($"We sent {i} messages to the queue.");
}
finally
{
    // Calling DisposeAsync on client types is required to ensure that network
    // resources and other unmanaged objects are properly cleaned up.
    await sender.DisposeAsync();
    await client.DisposeAsync();
}

Console.WriteLine("Press any key to end the application");
Console.ReadKey();
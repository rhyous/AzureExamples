﻿using Azure.Identity;
using Azure.Messaging.ServiceBus;

// the client that owns the connection and can be used to create senders and receivers
ServiceBusClient client;

// the processor that reads and processes messages from the queue
ServiceBusProcessor processor;

// The Service Bus client types are safe to cache and use as a singleton for the lifetime
// of the application, which is best practice when messages are being published or read
// regularly.
//
// Set the transport type to AmqpWebSockets so that the ServiceBusClient uses port 443.
// If you use the default AmqpTcp, make sure that ports 5671 and 5672 are open.

// TODO: Replace the <NAMESPACE-NAME> and <QUEUE-NAME> placeholders
var clientOptions = new ServiceBusClientOptions()
{
    TransportType = ServiceBusTransportType.AmqpWebSockets
};
client = new ServiceBusClient("rhyous-learning.servicebus.windows.net",
    new DefaultAzureCredential(), clientOptions);

// create a processor that we can use to process the messages
// TODO: Replace the <QUEUE-NAME> placeholder
processor = client.CreateProcessor("myqueue", new ServiceBusProcessorOptions());

try
{
    // add handler to process messages
    processor.ProcessMessageAsync += MessageHandler;

    // add handler to process any errors
    processor.ProcessErrorAsync += ErrorHandler;

    // start processing 
    await processor.StartProcessingAsync();

    Console.WriteLine("Run the sender the same time as this application and send a few messages.");
    Console.WriteLine("Press any key to exit this client.");
    Console.ReadKey();

    // stop processing 
    Console.WriteLine("\nStopping the receiver...");
    await processor.StopProcessingAsync();
    Console.WriteLine("Stopped receiving messages");
}
finally
{
    // Calling DisposeAsync on client types is required to ensure that network
    // resources and other unmanaged objects are properly cleaned up.
    await processor.DisposeAsync();
    await client.DisposeAsync();
}

// handle received messages
async Task MessageHandler(ProcessMessageEventArgs args)
{
    string body = args.Message.Body.ToString();
    Console.WriteLine($"Received: {body}");

    // complete the message. message is deleted from the queue. 
    await args.CompleteMessageAsync(args.Message);
}

// handle any errors when receiving messages
Task ErrorHandler(ProcessErrorEventArgs args)
{
    Console.WriteLine(args.Exception.ToString());
    return Task.CompletedTask;
}
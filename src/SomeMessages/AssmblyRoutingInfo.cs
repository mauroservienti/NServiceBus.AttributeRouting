using NServiceBus.AttributeRouting;

[assembly: Route(commandsTo: "ReceiverEndpoint", messagesTo: "MessageReceiverEndpoint")]
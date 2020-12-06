using NServiceBus.AttributeRouting.Contracts;

[assembly: Route(commandsTo: "ReceiverEndpoint", messagesTo: "MessageReceiverEndpoint")]
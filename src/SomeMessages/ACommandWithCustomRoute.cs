using NServiceBus.AttributeRouting.Contracts;

namespace SomeMessages
{
    [RouteTo("AnotherReceiverEndpoint")]
    public class ACommandWithCustomRoute
    {
    }
}
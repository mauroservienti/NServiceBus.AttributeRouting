using NServiceBus.AttributeRouting;

namespace SomeMessages
{
    [RouteTo("AnotherReceiverEndpoint")]
    public class ACommandWithCustomRoute
    {
    }
}
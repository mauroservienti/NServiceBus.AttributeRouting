using NServiceBus.AttributeRouting.Contracts;

namespace Messages
{
    [RouteTo("Receiver")]
    public class Sample
    {
    }
}
using NServiceBus.AttributeRouting;

namespace Messages
{
    [RouteTo("Receiver")]
    public class Sample
    {
    }
}
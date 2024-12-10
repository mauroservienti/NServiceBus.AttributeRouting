using NServiceBus;

namespace Receiver
{
    public class ReceiverConfiguration : EndpointConfiguration
    {
        public ReceiverConfiguration() : base("Receiver")
        {
            this.SendFailedMessagesTo("error");
            this.UseTransport<LearningTransport>();
            this.UseSerialization<SystemJsonSerializer>();
        }
    }
}
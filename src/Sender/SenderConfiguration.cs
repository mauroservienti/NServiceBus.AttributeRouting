using NServiceBus;

namespace Sender
{
    public class SenderConfiguration : EndpointConfiguration
    {
        public SenderConfiguration() : base("Sender")
        {
            this.SendFailedMessagesTo("error");
            this.Conventions()
                .DefiningMessagesAs(t => t.Namespace != null && t.Namespace.Equals("Messages"));
            
            this.UseTransport<LearningTransport>();
            this.UseAttributeRouting();
        }
    }
}
using System;
using System.Threading.Tasks;
using Messages;
using NServiceBus;

namespace Sender
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var endpointConfig = new EndpointConfiguration("Sender");
            endpointConfig.SendFailedMessagesTo("error");
            endpointConfig.Conventions()
                .DefiningMessagesAs(t => t.Namespace != null && t.Namespace.Equals("Messages"));
            
            endpointConfig.UseTransport<LearningTransport>();
            endpointConfig.UseAttributeRouting();

            var endpointInstance = await Endpoint.Start(endpointConfig);
            await endpointInstance.Send(new Sample());

            await endpointInstance.Stop();
        }
    }
}
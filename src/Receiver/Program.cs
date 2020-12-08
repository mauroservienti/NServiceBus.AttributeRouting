using System;
using System.Threading.Tasks;
using NServiceBus;

namespace Receiver
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var endpointConfig = new EndpointConfiguration("Receiver");
            endpointConfig.SendFailedMessagesTo("error");
            
            endpointConfig.UseTransport<LearningTransport>();
            
            var endpointInstance = await Endpoint.Start(endpointConfig);

            Console.ReadLine();
        }
    }
}
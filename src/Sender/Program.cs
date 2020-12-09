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
            var endpointInstance = await Endpoint.Start(new SenderConfiguration());
            await endpointInstance.Send(new Sample());
            await endpointInstance.Send(new AnotherSample());

            await endpointInstance.Stop();
        }
    }
}
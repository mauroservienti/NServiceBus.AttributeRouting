using System;
using System.Threading.Tasks;
using NServiceBus;

namespace Receiver
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var endpointInstance = await Endpoint.Start(new ReceiverConfiguration());

            Console.ReadLine();
        }
    }
}
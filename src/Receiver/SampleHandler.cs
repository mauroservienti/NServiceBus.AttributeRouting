using System;
using System.Threading.Tasks;
using Messages;
using NServiceBus;

namespace Receiver
{
    public class SampleHandler : IHandleMessages<Sample>
    {
        public Task Handle(Sample message, IMessageHandlerContext context)
        {
            Console.WriteLine($"Sample received: {context.MessageId}");
            return Task.CompletedTask;
        }
    }
}
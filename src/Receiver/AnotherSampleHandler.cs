using System;
using System.Threading.Tasks;
using Messages;
using NServiceBus;

namespace Receiver
{
    public class AnotherSampleHandler : IHandleMessages<AnotherSample>
    {
        public Task Handle(AnotherSample message, IMessageHandlerContext context)
        {
            Console.WriteLine($"AnotherSample received: {context.MessageId}");
            return Task.CompletedTask;
        }
    }
}
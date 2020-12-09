using System.Threading.Tasks;
using NServiceBus.AcceptanceTesting;
using NServiceBus.IntegrationTesting;
using NUnit.Framework;
using Sender;
using NServiceBus;
using Receiver;

namespace Sample.IntegrationTests
{
    public class When_sender_sends_messages
    {
        [Test]
        public async Task Receiver_receives_messages()
        {
            var context = await Scenario.Define<IntegrationScenarioContext>()
                .WithEndpoint<SenderEndpoint>(behavior =>
                {
                    behavior.When(async session =>
                    {
                        await session.Send(new Messages.Sample());
                        await session.Send(new Messages.AnotherSample());
                    });
                })
                .WithEndpoint<ReceiverEndpoint>()
                .Done(c => (
                    c.MessageWasProcessed<Messages.Sample>() 
                    && c.MessageWasProcessed<Messages.AnotherSample>()
                ) || c.HasFailedMessages())
                .Run();

            Assert.True(context.MessageWasProcessedByHandler<Messages.Sample, Receiver.SampleHandler>());
            Assert.True(context.MessageWasProcessedByHandler<Messages.AnotherSample, Receiver.AnotherSampleHandler>());
            
            Assert.False(context.HasFailedMessages());
            Assert.False(context.HasHandlingErrors());
        }

        class SenderEndpoint : EndpointConfigurationBuilder
        {
            public SenderEndpoint()
            {
                EndpointSetup<EndpointTemplate<SenderConfiguration>>();
            }
        }

        class ReceiverEndpoint : EndpointConfigurationBuilder
        {
            public ReceiverEndpoint()
            {
                EndpointSetup<EndpointTemplate<ReceiverConfiguration>>();
            }
        }
    }
}
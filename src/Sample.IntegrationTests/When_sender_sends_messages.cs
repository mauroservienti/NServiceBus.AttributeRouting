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

            Assert.That(context.MessageWasProcessedByHandler<Messages.Sample, Receiver.SampleHandler>(), Is.True);
            Assert.That(context.MessageWasProcessedByHandler<Messages.AnotherSample, Receiver.AnotherSampleHandler>(), Is.True);
            
            Assert.That(context.HasFailedMessages(), Is.False);
            Assert.That(context.HasHandlingErrors(), Is.False);
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
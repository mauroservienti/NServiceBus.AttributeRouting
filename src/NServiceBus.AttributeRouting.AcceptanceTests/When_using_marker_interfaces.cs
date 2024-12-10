using NServiceBus.AcceptanceTesting;
using NUnit.Framework;
using System.Threading.Tasks;

namespace NServiceBus.AttributeRouting.AcceptanceTests
{
    public class When_using_marker_interfaces
    {
        [Test]
        public async Task route_to_attribute_should_be_respected()
        {
            var context = await Scenario.Define<Context>()
                .WithEndpoint<SenderEndpoint>(g => g.When(b => b.Send(new MessageWithMarkerInterface())))
                .WithEndpoint<ReceiverEndpoint>()
                .Done(c => c.MessageReceived)
                .Run();

            Assert.That(context.MessageReceived, Is.True);
        }

        class Context : ScenarioContext
        {
            public bool MessageReceived { get; set; }
        }

        class SenderEndpoint : EndpointConfigurationBuilder
        {
            public SenderEndpoint()
            {
                EndpointSetup<DefaultServer>(config=>config.UseAttributeRouting());
            }
        }

        class ReceiverEndpoint : EndpointConfigurationBuilder
        {
            public ReceiverEndpoint()
            {
                EndpointSetup<DefaultServer>();
            }

            class Handler(Context TestContext) : IHandleMessages<MessageWithMarkerInterface>
            {
                public Task Handle(MessageWithMarkerInterface message, IMessageHandlerContext context)
                {
                    TestContext.MessageReceived = true;

                    return Task.FromResult(0);
                }
            }
        }

        [RouteTo("ReceiverEndpoint")]
        public class MessageWithMarkerInterface : IMessage
        {
        }
    }
}

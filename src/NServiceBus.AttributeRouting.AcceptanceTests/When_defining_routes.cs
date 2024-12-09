using NServiceBus.AcceptanceTesting;
using NUnit.Framework;
using System.Threading.Tasks;

namespace NServiceBus.AttributeRouting.AcceptanceTests
{
    public class When_defining_routes
    {
        [Test, Explicit]
        public async Task route_to_attribute_should_be_overwritten()
        {
            var context = await Scenario.Define<Context>()
                .WithEndpoint<SenderEndpoint>(g => g.When(b => b.Send(new Message())))
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
                EndpointSetup<DefaultServer>(config =>
                {
                    var routingSettings = config.UseTransport(new AcceptanceTestingTransport()
                    {
                        StorageLocation = StorageUtils.GetAcceptanceTestingTransportStorageDirectory()
                    });
                    
                    routingSettings.RouteToEndpoint(typeof(Message), "ReceiverEndpoint");

                    config.Conventions().DefiningMessagesAs(t => t == typeof(Message));
                    config.UseAttributeRouting();
                });
            }
        }

        class ReceiverEndpoint : EndpointConfigurationBuilder
        {
            public ReceiverEndpoint()
            {
                EndpointSetup<DefaultServer>(config =>
                {
                    config.Conventions().DefiningMessagesAs(t => t == typeof(Message));
                });
            }

            class Handler(Context TestContext) : IHandleMessages<Message>
            {
                public Task Handle(Message message, IMessageHandlerContext context)
                {
                    TestContext.MessageReceived = true;

                    return Task.FromResult(0);
                }
            }
        }

        [RouteTo("ThisWillBeOverwritten")]
        public class Message
        {
        }
    }
}

using NServiceBus.AcceptanceTesting;
using System.Threading.Tasks;
using Xunit;

namespace NServiceBus.AttributeRouting.AcceptanceTests
{
    public class When_using_marker_interfaces
    {
        [Fact]
        public async Task route_to_attribute_should_be_respected()
        {
            var context = await Scenario.Define<Context>()
                .WithEndpoint<When_using_marker_interfaces_SenderEndpoint>(g => g.When(b => b.Send(new MessageWithMarkerInterface())))
                .WithEndpoint<When_using_marker_interfaces_ReceiverEndpoint>()
                .Done(c => c.MessageReceived)
                .Run();

            Assert.True(context.MessageReceived);
        }

        class Context : ScenarioContext
        {
            public bool MessageReceived { get; set; }
        }

        class When_using_marker_interfaces_SenderEndpoint : EndpointConfigurationBuilder
        {
            public When_using_marker_interfaces_SenderEndpoint()
            {
                EndpointSetup<DefaultServer>(config=>config.EnableAttributeRouting());
            }
        }

        class When_using_marker_interfaces_ReceiverEndpoint : EndpointConfigurationBuilder
        {
            public When_using_marker_interfaces_ReceiverEndpoint()
            {
                EndpointSetup<DefaultServer>();
            }

            class Handler : IHandleMessages<MessageWithMarkerInterface>
            {
                public Context TestContext { get; set; }

                public Task Handle(MessageWithMarkerInterface message, IMessageHandlerContext context)
                {
                    TestContext.MessageReceived = true;

                    return Task.FromResult(0);
                }
            }
        }

        [RouteTo("When_using_marker_interfaces_ReceiverEndpoint")]
        public class MessageWithMarkerInterface : IMessage
        {
        }
    }
}

using NServiceBus.AcceptanceTesting;
using System.Threading.Tasks;
using Xunit;

namespace NServiceBus.AttributeRouting.AcceptanceTests
{
    public class When_using_conventions
    {
        [Fact]
        public async Task route_to_attribute_should_be_respected()
        {
            var context = await Scenario.Define<Context>()
                .WithEndpoint<When_using_conventions_SenderEndpoint>(g => g.When(b => b.Send(new AConventionBasedMessage())))
                .WithEndpoint<When_using_conventions_ReceiverEndpoint>()
                .Done(c => c.MessageReceived)
                .Run();

            Assert.True(context.MessageReceived);
        }

        class Context : ScenarioContext
        {
            public bool MessageReceived { get; set; }
        }

        class When_using_conventions_SenderEndpoint : EndpointConfigurationBuilder
        {
            public When_using_conventions_SenderEndpoint()
            {
                EndpointSetup<DefaultServer>(config =>
                {
                    config.Conventions().DefiningMessagesAs(t => t == typeof(AConventionBasedMessage));
                    config.EnableAttributeRouting();
                });
            }
        }

        class When_using_conventions_ReceiverEndpoint : EndpointConfigurationBuilder
        {
            public When_using_conventions_ReceiverEndpoint()
            {
                EndpointSetup<DefaultServer>(config =>
                {
                    config.Conventions().DefiningMessagesAs(t => t == typeof(AConventionBasedMessage));
                });
            }

            class Handler : IHandleMessages<AConventionBasedMessage>
            {
                public Context TestContext { get; set; }

                public Task Handle(AConventionBasedMessage message, IMessageHandlerContext context)
                {
                    TestContext.MessageReceived = true;

                    return Task.FromResult(0);
                }
            }
        }

        [RouteTo("When_using_conventions_ReceiverEndpoint")]
        public class AConventionBasedMessage
        {
        }
    }
}

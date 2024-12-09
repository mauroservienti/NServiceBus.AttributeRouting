using NServiceBus.AcceptanceTesting;
using NUnit.Framework;
using System.Threading.Tasks;
using SomeMessages;

namespace NServiceBus.AttributeRouting.AcceptanceTests
{
    public class When_using_assembly_level_routing
    {
        [Test]
        public async Task assembly_level_route_attribute_should_be_respected()
        {
            var context = await Scenario.Define<Context>()
                .WithEndpoint<SenderEndpoint>(g => g.When(b => b.Send(new ACommand())))
                .WithEndpoint<ReceiverEndpoint>()
                .Done(c => c.CommandReceived)
                .Run();

            Assert.That(context.CommandReceived, Is.True);
        }

        class Context : ScenarioContext
        {
            public bool CommandReceived { get; set; }
        }

        class SenderEndpoint : EndpointConfigurationBuilder
        {
            public SenderEndpoint()
            {
                EndpointSetup<DefaultServer>((config, descriptor) =>
                {
                    config.Conventions().DefiningCommandsAs(t => t == typeof(ACommand));
                    config.UseAttributeRouting();
                }).IncludeType<ACommand>();
            }
        }

        class ReceiverEndpoint : EndpointConfigurationBuilder
        {
            public ReceiverEndpoint()
            {
                EndpointSetup<DefaultServer>((config, descriptor) =>
                {
                    config.Conventions().DefiningCommandsAs(t => t == typeof(ACommand));
                }).IncludeType<ACommand>();
            }

            class Handler(Context TestContext) : IHandleMessages<ACommand>
            {
                public Task Handle(ACommand message, IMessageHandlerContext context)
                {
                    TestContext.CommandReceived = true;

                    return Task.FromResult(0);
                }
            }
        }
    }
}

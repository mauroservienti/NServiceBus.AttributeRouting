using NServiceBus.AcceptanceTesting;
using NUnit.Framework;
using System.Threading.Tasks;
using SomeMessages;

namespace NServiceBus.AttributeRouting.AcceptanceTests
{
    public class When_using_assembly_level_routing_with_local_override
    {
        [Test]
        public async Task assembly_level_route_attribute_is_overridden_by_class_level_route_attribute()
        {
            var context = await Scenario.Define<Context>()
                .WithEndpoint<SenderEndpoint>(g => g.When(b => b.Send(new ACommandWithCustomRoute())))
                .WithEndpoint<AnotherReceiverEndpoint>()
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
                    config.Conventions().DefiningCommandsAs(t => t == typeof(ACommandWithCustomRoute));
                    config.UseAttributeRouting();
                }).IncludeType<ACommandWithCustomRoute>();
            }
        }

        class AnotherReceiverEndpoint : EndpointConfigurationBuilder
        {
            public AnotherReceiverEndpoint()
            {
                EndpointSetup<DefaultServer>((config, descriptor) =>
                {
                    config.Conventions().DefiningCommandsAs(t => t == typeof(ACommandWithCustomRoute));
                }).IncludeType<ACommandWithCustomRoute>();
            }

            class Handler(Context TestContext) : IHandleMessages<ACommandWithCustomRoute>
            {
                public Task Handle(ACommandWithCustomRoute message, IMessageHandlerContext context)
                {
                    TestContext.CommandReceived = true;

                    return Task.FromResult(0);
                }
            }
        }
    }
}

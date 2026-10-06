using NServiceBus.AcceptanceTesting;
using NUnit.Framework;
using System.Threading.Tasks;
using SomeMessages;

namespace NServiceBus.AttributeRouting.AcceptanceTests
{
    // SomeMessages doesn't reference NServiceBus, so assembly scanning skips it.
    // The sender endpoints deliberately don't include message types in the scan
    // to make sure routes are resolved for types NServiceBus doesn't know at startup.
    public class When_message_types_are_not_scanned
    {
        [Test]
        public async Task route_to_attribute_should_be_respected()
        {
            var context = await Scenario.Define<Context>()
                .WithEndpoint<SenderEndpoint>(g => g.When(b => b.Send(new ACommandWithCustomRoute())))
                .WithEndpoint<AnotherReceiverEndpoint>()
                .Done(c => c.CommandWithCustomRouteReceived)
                .Run();

            Assert.That(context.CommandWithCustomRouteReceived, Is.True);
        }

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
            public bool CommandWithCustomRouteReceived { get; set; }
        }

        class SenderEndpoint : EndpointConfigurationBuilder
        {
            public SenderEndpoint()
            {
                EndpointSetup<DefaultServer>(config =>
                {
                    config.Conventions().DefiningCommandsAs(t => t.Namespace == typeof(ACommand).Namespace);
                    config.UseAttributeRouting();
                    config.SendOnly();
                });
            }
        }

        class ReceiverEndpoint : EndpointConfigurationBuilder
        {
            public ReceiverEndpoint()
            {
                EndpointSetup<DefaultServer>(config =>
                {
                    config.Conventions().DefiningCommandsAs(t => t.Namespace == typeof(ACommand).Namespace);
                });
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

        class AnotherReceiverEndpoint : EndpointConfigurationBuilder
        {
            public AnotherReceiverEndpoint()
            {
                EndpointSetup<DefaultServer>(config =>
                {
                    config.Conventions().DefiningCommandsAs(t => t.Namespace == typeof(ACommand).Namespace);
                });
            }

            class Handler(Context TestContext) : IHandleMessages<ACommandWithCustomRoute>
            {
                public Task Handle(ACommandWithCustomRoute message, IMessageHandlerContext context)
                {
                    TestContext.CommandWithCustomRouteReceived = true;

                    return Task.FromResult(0);
                }
            }
        }
    }
}

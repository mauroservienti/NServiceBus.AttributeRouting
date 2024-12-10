using NServiceBus.AcceptanceTesting;
using NUnit.Framework;
using System.Threading.Tasks;
using SomeMessages;

namespace NServiceBus.AttributeRouting.AcceptanceTests
{
    public class When_using_assembly_level_routing_mixing_commands_and_messages_and_with_local_override
    {
        [Test]
        public async Task assembly_level_route_attribute_is_overridden_by_class_level_route_attribute()
        {
            var context = await Scenario.Define<Context>()
                .WithEndpoint<SenderEndpoint>(g => g.When(async b =>
                {
                    await b.Send(new AMessage());
                    await b.Send(new ACommand());
                    await b.Send(new ACommandWithCustomRoute());
                }))
                .WithEndpoint<ReceiverEndpoint>()
                .WithEndpoint<MessageReceiverEndpoint>()
                .WithEndpoint<AnotherReceiverEndpoint>()
                .Done(c => c.ACommandWithCustomRouteReceived && c.ACommandReceived && c.AMessageReceived)
                .Run();

            Assert.That(context.AMessageReceived, Is.True);
            Assert.That(context.ACommandReceived, Is.True);
            Assert.That(context.ACommandWithCustomRouteReceived, Is.True);
        }

        class Context : ScenarioContext
        {
            public bool AMessageReceived { get; set; }
            public bool ACommandReceived { get; set; }
            public bool ACommandWithCustomRouteReceived { get; set; }
        }

        class SenderEndpoint : EndpointConfigurationBuilder
        {
            public SenderEndpoint()
            {
                EndpointSetup<DefaultServer>((config, descriptor) =>
                {
                    config.Conventions().DefiningMessagesAs(t => t== typeof(AMessage));
                    config.Conventions().DefiningCommandsAs(t => t.Name.StartsWith("ACommand"));
                    config.UseAttributeRouting();
                }).IncludeType<ACommand>().IncludeType<AMessage>().IncludeType<ACommandWithCustomRoute>();
            }
        }
        
        class ReceiverEndpoint : EndpointConfigurationBuilder
        {
            public ReceiverEndpoint()
            {
                EndpointSetup<DefaultServer>().IncludeType<ACommand>();
            }

            class Handler(Context TestContext) : IHandleMessages<ACommand>
            {
                public Task Handle(ACommand message, IMessageHandlerContext context)
                {
                    TestContext.ACommandReceived = true;

                    return Task.FromResult(0);
                }
            }
        }

        class AnotherReceiverEndpoint : EndpointConfigurationBuilder
        {
            public AnotherReceiverEndpoint()
            {
                EndpointSetup<DefaultServer>().IncludeType<ACommandWithCustomRoute>();
            }

            class Handler(Context TestContext) : IHandleMessages<ACommandWithCustomRoute>
            {
                public Task Handle(ACommandWithCustomRoute message, IMessageHandlerContext context)
                {
                    TestContext.ACommandWithCustomRouteReceived = true;

                    return Task.FromResult(0);
                }
            }
        }
        
        class MessageReceiverEndpoint : EndpointConfigurationBuilder
        {
            public MessageReceiverEndpoint()
            {
                EndpointSetup<DefaultServer>().IncludeType<AMessage>();
            }

            class Handler(Context TestContext) : IHandleMessages<AMessage>
            {
                public Task Handle(AMessage message, IMessageHandlerContext context)
                {
                    TestContext.AMessageReceived = true;

                    return Task.FromResult(0);
                }
            }
        }
    }
}

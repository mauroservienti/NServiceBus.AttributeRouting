using NServiceBus.AcceptanceTesting;
using NUnit.Framework;
using System.Threading.Tasks;
using SomeMessages;

namespace NServiceBus.AttributeRouting.AcceptanceTests
{
    public class When_using_assembly_level_routing_and_mixing_commands_and_messages
    {
        [Test]
        public async Task assembly_level_route_attribute_should_be_respected()
        {
            var context = await Scenario.Define<Context>()
                .WithEndpoint<SenderEndpoint>(g => g.When(async b =>
                {
                    await b.Send(new ACommand());
                    await b.Send(new AMessage());
                }))
                .WithEndpoint<ReceiverEndpoint>()
                .WithEndpoint<MessageReceiverEndpoint>()
                .Done(c => c.CommandReceived && c.MessageReceived)
                .Run();

            Assert.That(context.CommandReceived, Is.True);
            Assert.That(context.MessageReceived, Is.True);
        }

        class Context : ScenarioContext
        {
            public bool CommandReceived { get; set; }
            public bool MessageReceived { get; set; }
        }

        class SenderEndpoint : EndpointConfigurationBuilder
        {
            public SenderEndpoint()
            {
                EndpointSetup<DefaultServer>((config, descriptor) =>
                {
                    config.Conventions().DefiningMessagesAs(t => t == typeof(AMessage));
                    config.Conventions().DefiningCommandsAs(t => t == typeof(ACommand));
                    config.UseAttributeRouting();
                }).IncludeType<ACommand>().IncludeType<AMessage>();
            }
        }

        class ReceiverEndpoint : EndpointConfigurationBuilder
        {
            public ReceiverEndpoint()
            {
                EndpointSetup<DefaultServer>((config, descriptor) =>
                {
                    config.Conventions().DefiningCommandsAs(t => t == typeof(ACommand));
                    config.Conventions().DefiningMessagesAs(t => t == typeof(AMessage));
                }).IncludeType<ACommand>().IncludeType<AMessage>();
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
        
        class MessageReceiverEndpoint : EndpointConfigurationBuilder
        {
            public MessageReceiverEndpoint()
            {
                EndpointSetup<DefaultServer>((config, descriptor) =>
                {
                    config.Conventions().DefiningCommandsAs(t => t == typeof(ACommand));
                    config.Conventions().DefiningMessagesAs(t => t == typeof(AMessage));
                }).IncludeType<ACommand>().IncludeType<AMessage>();
            }

            class Handler(Context TestContext) : IHandleMessages<AMessage>
            {
                public Task Handle(AMessage message, IMessageHandlerContext context)
                {
                    TestContext.MessageReceived = true;

                    return Task.FromResult(0);
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using NServiceBus.AcceptanceTesting;
using NServiceBus.AttributeRouting.Contracts;
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

            Assert.True(context.AMessageReceived);
            Assert.True(context.ACommandReceived);
            Assert.True(context.ACommandWithCustomRouteReceived);
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
                EndpointSetup<ServerWithSomeMessages>((config, descriptor) =>
                {
                    config.Conventions().DefiningMessagesAs(t => t== typeof(AMessage));
                    config.Conventions().DefiningCommandsAs(t => t.Name.StartsWith("ACommand"));
                    config.UseAttributeRouting();
                });
            }
        }
        
        class ReceiverEndpoint : EndpointConfigurationBuilder
        {
            public ReceiverEndpoint()
            {
                EndpointSetup<DefaultServer>();
            }

            class Handler : IHandleMessages<ACommand>
            {
                public Context TestContext { get; set; }

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
                EndpointSetup<DefaultServer>();
            }

            class Handler : IHandleMessages<ACommandWithCustomRoute>
            {
                public Context TestContext { get; set; }

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
                EndpointSetup<DefaultServer>();
            }

            class Handler : IHandleMessages<AMessage>
            {
                public Context TestContext { get; set; }

                public Task Handle(AMessage message, IMessageHandlerContext context)
                {
                    TestContext.AMessageReceived = true;

                    return Task.FromResult(0);
                }
            }
        }
    }
}

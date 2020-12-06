using System;
using System.Collections.Generic;
using NServiceBus.AcceptanceTesting;
using NServiceBus.AttributeRouting.Contracts;
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

            Assert.True(context.CommandReceived);
        }

        class Context : ScenarioContext
        {
            public bool CommandReceived { get; set; }
        }

        class SenderEndpoint : EndpointConfigurationBuilder
        {
            public SenderEndpoint()
            {
                EndpointSetup<ServerWithSomeMessages>((config, descriptor) =>
                {
                    config.Conventions().DefiningCommandsAs(t => t == typeof(ACommand));
                    config.UseAttributeRouting();
                });
            }
        }

        class ReceiverEndpoint : EndpointConfigurationBuilder
        {
            public ReceiverEndpoint()
            {
                EndpointSetup<DefaultServer>((config, descriptor) =>
                {
                    config.Conventions().DefiningCommandsAs(t => t == typeof(ACommand));
                });
            }

            class Handler : IHandleMessages<ACommand>
            {
                public Context TestContext { get; set; }

                public Task Handle(ACommand message, IMessageHandlerContext context)
                {
                    TestContext.CommandReceived = true;

                    return Task.FromResult(0);
                }
            }
        }
    }
}

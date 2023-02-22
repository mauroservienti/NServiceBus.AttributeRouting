using System;
using NServiceBus.AcceptanceTesting;
using NServiceBus.AttributeRouting.Contracts;
using NUnit.Framework;
using System.Threading.Tasks;

namespace NServiceBus.AttributeRouting.AcceptanceTests
{
    public class When_using_custom_conventions
    {
        [Test]
        public async Task route_to_attribute_should_be_respected()
        {
            var context = await Scenario.Define<Context>()
                .WithEndpoint<SenderEndpoint>(g => g.When(b => b.Send(new AConventionBasedMessage())))
                .WithEndpoint<ReceiverEndpoint>()
                .Done(c => c.MessageReceived)
                .Run();

            Assert.True(context.MessageReceived);
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
                    config.Conventions().Add(new MyConventions());
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
                    config.Conventions().Add(new MyConventions());
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

        [RouteTo("ReceiverEndpoint")]
        public class AConventionBasedMessage
        {
        }
        
        public class MyConventions : IMessageConvention
        {
            public bool IsMessageType(Type type) => type == typeof(AConventionBasedMessage);

            public bool IsCommandType(Type type) => false;

            public bool IsEventType(Type type) => false;

            public string Name { get; } = "My convention";
        }
    }
}

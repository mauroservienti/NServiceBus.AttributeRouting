using NServiceBus.AcceptanceTesting.Customization;
using NServiceBus.AcceptanceTesting.Support;
using NServiceBus.Features;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NServiceBus.AttributeRouting.AcceptanceTests
{
    public class DefaultServer : IEndpointSetupTemplate
    {
        public DefaultServer()
        {
            typesToInclude = new List<Type>();
        }

        public DefaultServer(List<Type> typesToInclude)
        {
            this.typesToInclude = typesToInclude;
        }

        public Task<EndpointConfiguration> GetConfiguration(RunDescriptor runDescriptor, EndpointCustomizationConfiguration endpointConfiguration, Action<EndpointConfiguration> configurationBuilderCustomization)
        {
            var types = endpointConfiguration.GetTypesScopedByTestClass();

            typesToInclude.AddRange(types);

            var configuration = new EndpointConfiguration(endpointConfiguration.EndpointName);

            configuration.TypesToIncludeInScan(typesToInclude);
            configuration.EnableInstallers();

            configuration.UseContainer(new AcceptanceTestingContainer());
            configuration.DisableFeature<TimeoutManager>();

            var recoverability = configuration.Recoverability();
            recoverability.Delayed(delayed => delayed.NumberOfRetries(0));
            recoverability.Immediate(immediate => immediate.NumberOfRetries(0));
            configuration.SendFailedMessagesTo("error");

            configuration.UseTransport<LearningTransport>();

            configuration.RegisterComponentsAndInheritanceHierarchy(runDescriptor);

            configuration.UsePersistence<LearningPersistence>();

            configurationBuilderCustomization(configuration);

            return Task.FromResult(configuration);
        }

        List<Type> typesToInclude;
    }
}

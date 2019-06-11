﻿using NServiceBus.AcceptanceTesting.Customization;
using NServiceBus.AcceptanceTesting.Support;
using NServiceBus.Features;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace NServiceBus.AttributeRouting.AcceptanceTests
{
    public class DefaultServer : IEndpointSetupTemplate
    {
        string storageDir;

        public DefaultServer()
            : this(new List<Type>())
        {

        }

        public DefaultServer(List<Type> typesToInclude)
        {
            this.typesToInclude = typesToInclude;

            storageDir = StorageUtils.GetAcceptanceTestingTransportStorageDirectory();
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

            var transportConfig = configuration.UseTransport<AcceptanceTestingTransport>()
                .StorageDirectory(storageDir);

            configuration.RegisterComponentsAndInheritanceHierarchy(runDescriptor);

            configurationBuilderCustomization(configuration);

            runDescriptor.OnTestCompleted(summary => 
            {
                if (Directory.Exists(storageDir))
                {
                    Directory.Delete(storageDir, true);
                }

                return Task.FromResult(0);
            });

            return Task.FromResult(configuration);
        }

        List<Type> typesToInclude;
    }
}

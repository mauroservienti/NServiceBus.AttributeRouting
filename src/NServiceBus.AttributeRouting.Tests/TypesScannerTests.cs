using System.Linq;
using ExcludedAssembly;
using NServiceBus.AttributeRouting.AssemblyScanning;
using NUnit.Framework;
using SomeMessages;

namespace NServiceBus.AttributeRouting.Tests
{
    public class TypesScannerTests
    {
        [Test]
        public void When_using_default_settings_all_messages_should_be_found()
        {
            var assemblyScannerConfig = new AssemblyScannerConfiguration();

            var conventions = new Conventions();
            conventions.Add(new DelegateMessageConvention()
            {
                IsCommandTypeConvention = t=>t.Name.EndsWith("Command"),
                IsMessageTypeConvention = t=>t.Name.EndsWith("Message")
            });

            var types = TypesScanner.ScanMessageTypes(assemblyScannerConfig, conventions).ToList();

            Assert.Contains(typeof(ACommand), types);
            Assert.Contains(typeof(AMessage), types);
            Assert.Contains(typeof(ThisIsAnExcludedCommand), types);
        }

        [Test]
        public void When_excluding_assembly_types_from_assembly_should_not_be_found()
        {
            var assemblyScannerConfig = new AssemblyScannerConfiguration();
            assemblyScannerConfig.ExcludeAssemblies("ExcludedAssembly.dll");

            var conventions = new Conventions();
            conventions.Add(new DelegateMessageConvention()
            {
                IsCommandTypeConvention = t=>t.Name.EndsWith("Command"),
                IsMessageTypeConvention = t=>t.Name.EndsWith("Message")
            });

            var types = TypesScanner.ScanMessageTypes(assemblyScannerConfig, conventions).ToList();

            Assert.Contains(typeof(ACommand), types);
            Assert.Contains(typeof(AMessage), types);
            Assert.False(types.Contains(typeof(ThisIsAnExcludedCommand)));
        }

        [Test]
        public void When_excluding_types_types_from_assembly_should_not_be_found()
        {
            var assemblyScannerConfig = new AssemblyScannerConfiguration();
            assemblyScannerConfig.ExcludeTypes(typeof(ThisIsAnExcludedCommand));

            var conventions = new Conventions();
            conventions.Add(new DelegateMessageConvention()
            {
                IsCommandTypeConvention = t=>t.Name.EndsWith("Command"),
                IsMessageTypeConvention = t=>t.Name.EndsWith("Message")
            });

            var types = TypesScanner.ScanMessageTypes(assemblyScannerConfig, conventions).ToList();

            Assert.Contains(typeof(ACommand), types);
            Assert.Contains(typeof(AMessage), types);
            Assert.False(types.Contains(typeof(ThisIsAnExcludedCommand)));
        }
    }
}
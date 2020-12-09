using System;
using NServiceBus.Routing;
using NUnit.Framework;
using System.Collections.Generic;
using System.Reflection;

namespace NServiceBus.AttributeRouting.Tests
{
    public class Given_that_attribute_routing_setup_relies_on_reflection
    {
        [Test]
        public void Make_sure_assembly_scanner_configuration_has_ExcludedAssemblies_property()
        {
            var property = typeof(AssemblyScannerConfiguration).GetProperty("ExcludedAssemblies",
                BindingFlags.Instance | BindingFlags.NonPublic);
            
            Assert.NotNull(property, "Cannot find ExcludedAssemblies property on AssemblyScannerConfiguration type");
            Assert.True(property.PropertyType==typeof(List<string>),"The return type of ExcludedAssemblies is not the expected List<string>.");
        }
        
        [Test]
        public void Make_sure_assembly_scanner_configuration_has_ExcludedTypes_property()
        {
            var property = typeof(AssemblyScannerConfiguration).GetProperty("ExcludedTypes",
                BindingFlags.Instance | BindingFlags.NonPublic);
            
            Assert.NotNull(property, "Cannot find ExcludedTypes property on AssemblyScannerConfiguration type");
            Assert.True(property.PropertyType==typeof(List<Type>),"The return type of ExcludedTypes is not the expected List<Type>.");
        }

        [Test]
        public void Make_sure_unicast_routing_table_GetRouteFor_doesnt_change()
        {
            var method = typeof(UnicastRoutingTable)
                .GetMethod("GetRouteFor", BindingFlags.Instance | BindingFlags.NonPublic);


            Assert.False(method == null, "Cannot find GetRouteFor method on UnicastRoutingTable type");
            Assert.True(method.ReturnType == typeof(UnicastRoute), "The return type of GetRouteFor is not the expected UnicastRoute.");
        }
    }
}

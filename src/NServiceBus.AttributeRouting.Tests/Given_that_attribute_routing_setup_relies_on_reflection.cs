using NServiceBus.Routing;
using NServiceBus.Unicast.Messages;
using NUnit.Framework;
using System.Collections.Generic;
using System.Reflection;

namespace NServiceBus.AttributeRouting.Tests
{
    public class Given_that_attribute_routing_setup_relies_on_reflection
    {
        [Test]
        public void Make_sure_unicast_routing_table_GetRouteFor_doesnt_change()
        {
            var method = typeof(UnicastRoutingTable)
                .GetMethod("GetRouteFor", BindingFlags.Instance | BindingFlags.NonPublic);


            Assert.False(method == null, "Cannot find GetRouteFor method on UnicastRoutingTable type");
            Assert.True(method.ReturnType == typeof(UnicastRoute), "The return type is not the expected UnicastRoute.");
        }
    }
}

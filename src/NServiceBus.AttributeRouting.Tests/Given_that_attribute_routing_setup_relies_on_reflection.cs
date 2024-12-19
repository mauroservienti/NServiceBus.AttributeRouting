using NServiceBus.Routing;
using NUnit.Framework;
using System.Collections.Generic;
using System.Reflection;
using NServiceBus.Unicast.Messages;

namespace NServiceBus.AttributeRouting.Tests
{
    public class Given_that_attribute_routing_setup_relies_on_reflection
    {
        [Test]
        public void Make_sure_unicast_routing_table_GetRouteFor_doesnt_change()
        {
            var method = typeof(UnicastRoutingTable)
                .GetMethod("GetRouteFor", BindingFlags.Instance | BindingFlags.NonPublic);


            Assert.That(method, Is.Not.Null, "Cannot find GetRouteFor method on UnicastRoutingTable type");
            Assert.That(method.ReturnType, Is.EqualTo(typeof(UnicastRoute)), "The return type of GetRouteFor is not the expected UnicastRoute.");
        }
    }
}

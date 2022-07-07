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
        public void Make_sure_message_metadata_registry_GetAllMessages_doesnt_change()
        {
            var method = typeof(MessageMetadataRegistry).GetMethod("GetAllMessages",
                BindingFlags.Instance | BindingFlags.NonPublic);
            
            Assert.False(method == null, "Cannot find GetAllMessages method on MessageMetadataRegistry type");
            Assert.True(method.ReturnType == typeof(IEnumerable<MessageMetadata>), "The return type of GetAllMessages is not the expected IEnumerable<MessageMetadata>.");
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

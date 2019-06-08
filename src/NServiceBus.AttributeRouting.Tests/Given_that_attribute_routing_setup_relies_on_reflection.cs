using NServiceBus.Unicast.Messages;
using NUnit.Framework;
using System.Collections.Generic;
using System.Reflection;

namespace NServiceBus.AttributeRouting.Tests
{
    public class Given_that_attribute_routing_setup_relies_on_reflection
    {
        [Test]
        public void Make_sure_message_metadata_registry_doesnt_change()
        {
            var method = typeof(MessageMetadataRegistry)
                .GetMethod("GetAllMessages", BindingFlags.Instance | BindingFlags.NonPublic);


            Assert.False(method == null, "Cannot find GetAllMessages method on MessageMetadataRegistry type");
            Assert.True(method.ReturnType == typeof(IEnumerable<MessageMetadata>), "The return type is not the expected IEnumerable<MessageMetadata>.");
        }
    }
}

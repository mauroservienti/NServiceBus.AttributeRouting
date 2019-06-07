using NServiceBus.Features;
using NServiceBus.Routing;
using NServiceBus.Unicast.Messages;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NServiceBus.AttributeRouting
{
    public class AttributeRoutingFeature : Feature
    {
        public AttributeRoutingFeature()
        {

        }

        protected override void Setup(FeatureConfigurationContext context)
        {
            var messageMetadataRegistry = context.Settings.Get<MessageMetadataRegistry>();
            var allMessageMetadata = (IEnumerable<MessageMetadata>)typeof(MessageMetadataRegistry)
                .GetMethod("GetAllMessages", BindingFlags.Instance | BindingFlags.NonPublic)
                .Invoke(messageMetadataRegistry, null);

            var routes = new List<RouteTableEntry>();

            foreach (var messageMetadata in allMessageMetadata)
            {
                var routeTo = messageMetadata.MessageType.GetCustomAttribute<RouteToAttribute>();
                if (routeTo != null)
                {
                    routes.Add(new RouteTableEntry(messageMetadata.MessageType, UnicastRoute.CreateFromEndpointName(routeTo.Destination)));
                }
            }

            if (routes.Any())
            {
                var routingTable = context.Settings.Get<UnicastRoutingTable>();
                routingTable.AddOrReplaceRoutes("AttributeRoutingSource", routes);
            }
        }
    }
}

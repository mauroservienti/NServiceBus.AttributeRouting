using NServiceBus.Features;
using NServiceBus.Routing;
using NServiceBus.Unicast.Messages;
using System;
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
            var unicastRoutingTable = context.Settings.Get<UnicastRoutingTable>();
            var messageMetadataRegistry = context.Settings.Get<MessageMetadataRegistry>();
            var allMessageMetadata = (IEnumerable<MessageMetadata>)typeof(MessageMetadataRegistry)
                .GetMethod("GetAllMessages", BindingFlags.Instance | BindingFlags.NonPublic)
                .Invoke(messageMetadataRegistry, null);

            var routes = new List<RouteTableEntry>();

            foreach (var messageMetadata in allMessageMetadata)
            {
                if (IsRouteDefinedFor(messageMetadata.MessageType, unicastRoutingTable))
                {
                    continue;
                }

                var routeTo = messageMetadata.MessageType.GetCustomAttribute<RouteToAttribute>();
                if (routeTo != null)
                {
                    routes.Add(new RouteTableEntry(messageMetadata.MessageType, UnicastRoute.CreateFromEndpointName(routeTo.Destination)));
                }
            }

            if (routes.Any())
            {
                unicastRoutingTable.AddOrReplaceRoutes("AttributeRoutingSource", routes);
            }
        }

        bool IsRouteDefinedFor(Type messageType, UnicastRoutingTable unicastRoutingTable)
        {
            //this could be a FastDelegate invocation
            var unicastRoute = (UnicastRoute)typeof(UnicastRoutingTable)
                .GetMethod("GetRouteFor", BindingFlags.Instance | BindingFlags.NonPublic)
                .Invoke(unicastRoutingTable, new[] { messageType });

            return unicastRoute != null;
        }
    }
}

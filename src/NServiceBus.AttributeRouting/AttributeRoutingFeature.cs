using NServiceBus.AttributeRouting.Contracts;
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
            var conventions = context.Settings.Get<Conventions>();
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
                    routes.Add(new RouteTableEntry(messageMetadata.MessageType,
                        UnicastRoute.CreateFromEndpointName(routeTo.Destination)));
                }
                else
                {
                    var route = messageMetadata.MessageType.Assembly.GetCustomAttribute<RouteAttribute>();
                    if (conventions.IsCommandType(messageMetadata.MessageType) 
                        && route.CommandsDestination != null)
                    {
                        routes.Add(new RouteTableEntry(messageMetadata.MessageType,
                            UnicastRoute.CreateFromEndpointName(route.CommandsDestination)));
                    }
                    else if (conventions.IsMessageType(messageMetadata.MessageType) 
                             && route.MessagesDestination != null)
                    {
                        routes.Add(new RouteTableEntry(messageMetadata.MessageType,
                            UnicastRoute.CreateFromEndpointName(route.MessagesDestination)));
                    }
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
                .Invoke(unicastRoutingTable, new[] {messageType});

            return unicastRoute != null;
        }
    }
}
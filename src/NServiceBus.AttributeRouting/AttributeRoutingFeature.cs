﻿using NServiceBus.AttributeRouting.Contracts;
using NServiceBus.Features;
using NServiceBus.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NServiceBus.AttributeRouting.AssemblyScanning;

namespace NServiceBus.AttributeRouting
{
    class AttributeRoutingFeature : Feature
    {
        public AttributeRoutingFeature()
        {
        }

        protected override void Setup(FeatureConfigurationContext context)
        {
            var coreScannerConfig = context.Settings.Get<AssemblyScannerConfiguration>();
            var unicastRoutingTable = context.Settings.Get<UnicastRoutingTable>();
            var conventions = context.Settings.Get<Conventions>();

            var messageTypes = TypesScanner.ScanMessageTypes(coreScannerConfig, conventions);

            var routes = new List<RouteTableEntry>();
            foreach (var messageType in messageTypes)
            {
                if (IsRouteDefinedFor(messageType, unicastRoutingTable))
                {
                    continue;
                }

                var routeToAttribute = messageType.GetCustomAttribute<RouteToAttribute>();
                if (routeToAttribute != null)
                {
                    routes.Add(new RouteTableEntry(messageType,
                        UnicastRoute.CreateFromEndpointName(routeToAttribute.Destination)));
                }
                else
                {
                    var routeAttribute = messageType.Assembly.GetCustomAttribute<RouteAttribute>();
                    if (conventions.IsCommandType(messageType)
                        && routeAttribute?.CommandsDestination != null)
                    {
                        routes.Add(new RouteTableEntry(messageType,
                            UnicastRoute.CreateFromEndpointName(routeAttribute.CommandsDestination)));
                    }
                    else if (conventions.IsMessageType(messageType)
                             && routeAttribute?.MessagesDestination != null)
                    {
                        routes.Add(new RouteTableEntry(messageType,
                            UnicastRoute.CreateFromEndpointName(routeAttribute.MessagesDestination)));
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
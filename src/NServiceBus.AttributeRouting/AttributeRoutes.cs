using NServiceBus.Routing;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace NServiceBus.AttributeRouting
{
    // Routes are resolved the first time a message type is sent, not at startup.
    // Message assemblies that don't reference NServiceBus are skipped by assembly
    // scanning, so their types are unknown to NServiceBus until they are sent.
    class AttributeRoutes
    {
        public AttributeRoutes(UnicastRoutingTable unicastRoutingTable, Conventions conventions)
        {
            this.unicastRoutingTable = unicastRoutingTable;
            this.conventions = conventions;

            var getRouteForMethodInfo = typeof(UnicastRoutingTable)
                .GetMethod("GetRouteFor", BindingFlags.Instance | BindingFlags.NonPublic);
            var messageTypeParameter = Expression.Parameter(typeof(Type), "messageType");
            var routingTableParameter = Expression.Parameter(typeof(UnicastRoutingTable), "routingTable");
            var methodCallExpression = Expression.Call(routingTableParameter, getRouteForMethodInfo, messageTypeParameter);
            getRouteFor = Expression.Lambda<Func<UnicastRoutingTable, Type, UnicastRoute>>(methodCallExpression, routingTableParameter, messageTypeParameter).Compile();
        }

        public void EnsureRouteFor(Type messageType)
        {
            if (resolvedMessageTypes.ContainsKey(messageType))
            {
                return;
            }

            lock (routesLock)
            {
                if (resolvedMessageTypes.ContainsKey(messageType))
                {
                    return;
                }

                // Routes defined by other sources, e.g. RouteToEndpoint, take precedence.
                if (getRouteFor(unicastRoutingTable, messageType) == null)
                {
                    var route = CreateRouteFor(messageType);
                    if (route != null)
                    {
                        routes.Add(new RouteTableEntry(messageType, route));
                        unicastRoutingTable.AddOrReplaceRoutes("AttributeRoutingSource", new List<RouteTableEntry>(routes));
                    }
                }

                resolvedMessageTypes[messageType] = true;
            }
        }

        UnicastRoute CreateRouteFor(Type messageType)
        {
            var routeToAttribute = messageType.GetCustomAttribute<RouteToAttribute>();
            if (routeToAttribute != null)
            {
                return UnicastRoute.CreateFromEndpointName(routeToAttribute.Destination);
            }

            var routeAttribute = messageType.Assembly.GetCustomAttribute<RouteAttribute>();
            if (conventions.IsCommandType(messageType)
                && routeAttribute?.CommandsDestination != null)
            {
                return UnicastRoute.CreateFromEndpointName(routeAttribute.CommandsDestination);
            }

            if (conventions.IsMessageType(messageType)
                && routeAttribute?.MessagesDestination != null)
            {
                return UnicastRoute.CreateFromEndpointName(routeAttribute.MessagesDestination);
            }

            return null;
        }

        readonly UnicastRoutingTable unicastRoutingTable;
        readonly Conventions conventions;
        readonly Func<UnicastRoutingTable, Type, UnicastRoute> getRouteFor;
        readonly ConcurrentDictionary<Type, bool> resolvedMessageTypes = new ConcurrentDictionary<Type, bool>();
        readonly List<RouteTableEntry> routes = new List<RouteTableEntry>();
        readonly object routesLock = new object();
    }
}

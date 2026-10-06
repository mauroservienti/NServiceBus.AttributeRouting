using NServiceBus.Features;
using NServiceBus.Routing;

namespace NServiceBus.AttributeRouting
{
    class AttributeRoutingFeature : Feature
    {
        protected override void Setup(FeatureConfigurationContext context)
        {
            var unicastRoutingTable = context.Settings.Get<UnicastRoutingTable>();
            var conventions = context.Settings.Get<Conventions>();

            var attributeRoutes = new AttributeRoutes(unicastRoutingTable, conventions);
            context.Pipeline.Register(new AttributeRoutingBehavior(attributeRoutes),
                "Adds routes defined using RouteTo and Route attributes to the routing table.");
        }
    }
}

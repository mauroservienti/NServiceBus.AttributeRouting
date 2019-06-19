using NServiceBus.AttributeRouting;

namespace NServiceBus
{
    public static class RoutingSettingsExtensions
    {
        public static void UseAttributeRouting(this EndpointConfiguration endpointConfiguration)
        {
            endpointConfiguration.EnableFeature<AttributeRoutingFeature>();
        }
    }
}

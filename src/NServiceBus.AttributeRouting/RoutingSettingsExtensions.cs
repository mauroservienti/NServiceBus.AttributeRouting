using NServiceBus.AttributeRouting;

namespace NServiceBus
{
    public static class RoutingSettingsExtensions
    {
        public static EndpointConfiguration UseAttributeRouting(this EndpointConfiguration endpointConfiguration)
        {
            endpointConfiguration.EnableFeature<AttributeRoutingFeature>();

            return endpointConfiguration;
        }
    }
}

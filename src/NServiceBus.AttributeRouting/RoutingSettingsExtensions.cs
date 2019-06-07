using NServiceBus.AttributeRouting;

namespace NServiceBus
{
    public static class RoutingSettingsExtensions
    {
        public static void EnableAttributeRouting(this EndpointConfiguration endpointConfiguration)
        {
            endpointConfiguration.EnableFeature<AttributeRoutingFeature>();
        }
    }
}

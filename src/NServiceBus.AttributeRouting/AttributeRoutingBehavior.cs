using NServiceBus.Pipeline;
using System;
using System.Threading.Tasks;

namespace NServiceBus.AttributeRouting
{
    class AttributeRoutingBehavior : Behavior<IOutgoingSendContext>
    {
        public AttributeRoutingBehavior(AttributeRoutes attributeRoutes)
        {
            this.attributeRoutes = attributeRoutes;
        }

        public override Task Invoke(IOutgoingSendContext context, Func<Task> next)
        {
            attributeRoutes.EnsureRouteFor(context.Message.MessageType);

            return next();
        }

        readonly AttributeRoutes attributeRoutes;
    }
}

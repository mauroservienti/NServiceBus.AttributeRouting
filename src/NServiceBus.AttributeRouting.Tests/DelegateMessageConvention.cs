using System;

namespace NServiceBus.AttributeRouting.Tests
{
    class DelegateMessageConvention : IMessageConvention
    {
        public Func<Type, bool> IsMessageTypeConvention { get; set; } = t => false;
        public Func<Type, bool> IsCommandTypeConvention { get; set; }= t => false;
        public Func<Type, bool> IsEventTypeConvention { get; set; } = t => false;

        public bool IsMessageType(Type type)
        {
            return IsMessageTypeConvention(type);
        }

        public bool IsCommandType(Type type)
        {
            return IsCommandTypeConvention(type);
        }

        public bool IsEventType(Type type)
        {
            return IsEventTypeConvention(type);
        }

        public string Name { get; } = "Test convention";
    }
}
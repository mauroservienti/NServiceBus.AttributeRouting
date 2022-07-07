using System;
using System.Collections.Generic;
using System.Reflection;
using NServiceBus.Unicast.Messages;

namespace NServiceBus.AttributeRouting
{
    static class MessageMetadataRegistryExtensions
    {
        public static IEnumerable<MessageMetadata> GetAllMessages(this MessageMetadataRegistry registry)
        {
            var methodInfo = typeof(MessageMetadataRegistry).GetMethod("GetAllMessages", BindingFlags.Instance | BindingFlags.NonPublic);
            return (IEnumerable<MessageMetadata>) methodInfo.Invoke(registry, Array.Empty<object>());
        }
    }
}
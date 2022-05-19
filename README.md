<img src="assets/icon.png" width="100" />

# NServiceBus.AttributeRouting

Enables to configure messages and commands routing by using attributes on message types:

```csharp
[RouteTo("DestinationEndpoint")]
public class SampleMessage
{}
```

> NOTE: Attributes are defined in a separate [NServiceBus.AttributeRouting.Contracts](https://github.com/mauroservienti/NServiceBus.AttributeRouting.Contracts) package to prevent coupling endpoints to the NServiceBus version this feature depends on.

and when configuring the endpoint:

```csharp
endpointConfiguration.UseAttributeRouting();
```

> NOTE: Only [Messages and Commands](https://docs.particular.net/nservicebus/messaging/messages-events-commands) are supported. In NServiceBus Events are treated differently based on the underlying transport capabilities: If the transport supports native pub/sub (e.g. RabbitMQ or Azure Service Bus) everything is handled automatically, otherwise publishers needs to be manually registered. As of now registering publishers using attributes is not supported.

## Assembly level routing

When using endpoint-oriented message assemblies, messages are grouped into assembly per endpoint. In such a scenario, commands and messages are generally treated as internal to the endpoint meaning that all messages and commands defined in a endpoint-oriented message assembly are routed to the same destination endpoint. It's possible to define routes at the assembly level:

```csharp
[assembly: Route(commandsTo: "destination-endpoint-for-commands", messagesTo: "destination-endpoint-for-messages")]
```

`commandsTo` and `messagesTo` are optional arguments. Commands are messages, meaning that by definining routes for messages, using `messagesTo`, will also define routes for commands. If `commandsTo` destination value is specified it'll have precedence over the `messagesTo` value.

## Routes override (deprecated)

Attributes based routes are applied after explicitely defined routes, this allows to define overrides for routes defined using attributes.

NOTE: As of `NServiceBus 7.3.0` routes override cannot be used anymore. `7.3.0` [introduced a behavior breaking change](https://github.com/Particular/NServiceBus/issues/5712) in the order in which components and features are registered and invoked causing routes override to stop working.

> Use case: an assembly containing a message decorated with the `RouteTo` attribute is distributed to endpoints. Later handlers topology change causing the message to end up at the wrong destination service.

In such a scenario it’s suggested to deploy a new version of the messages assembly, if this is not possibile on not doable in a timely fashion attribute based routes can be overwritten by explicitly defining routes for a given message. If `SampleMessage` is the message type for which a route override needs to be defined, endpoint configuration can be changed as follows:

```csharp
endpointConfiguration.UseTransport<YourChoice>()
   .Routing()
      .RouteToEndopoint( typeof( SampleMessage ), "NewDestination" );
endpointConfiguration.EnableAttributeRouting();
```

NewDestination will take precedence over DestinationEndpoint.

### Downloads

Nuget package: <https://www.nuget.org/packages/NServiceBus.AttributeRouting/>

---

Icon: [route](https://thenounproject.com/search/?q=route&i=1720675) by [revo250](https://thenounproject.com/revo125cc/) from [the Noun Project](https://thenounproject.com/)

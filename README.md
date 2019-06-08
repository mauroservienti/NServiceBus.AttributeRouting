<img src="assets/icon.png" width="100" />

# NServiceBus.AttributeRouting

Enables to configure messages and commands routing by using attributes on message types:

```
[RouteTo("DestinationEndpoint")]
public class SampleMessage
{}
```

and when configuring the endpoint:

```
endpointConfiguration.EnableAttributeRouting();
```

---

Nuget package: [https://www.nuget.org/packages/NServiceBus.AttributeRouting/](https://www.nuget.org/packages/NServiceBus.AttributeRouting/)

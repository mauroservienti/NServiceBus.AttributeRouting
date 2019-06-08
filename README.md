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

### Downloads

Nuget package: [https://www.nuget.org/packages/NServiceBus.AttributeRouting/](https://www.nuget.org/packages/NServiceBus.AttributeRouting/)

---

Icon: [route](https://thenounproject.com/search/?q=route&i=1720675) by [revo250](https://thenounproject.com/revo125cc/) from [the Noun Project](https://thenounproject.com/)

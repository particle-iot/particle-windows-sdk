<p align="center" ><img src="https://avatars0.githubusercontent.com/u/2348966?s=200&v=4" alt="Particle" title="Particle"></p>

# Particle Windows Cloud SDK

[![license](https://img.shields.io/hexpm/l/plug.svg)](https://github.com/spark/particle-windows-sdk/blob/master/LICENSE)
[![NuGet Version](http://img.shields.io/nuget/v/Particle.SDK.svg?style=flat)](https://www.nuget.org/packages/Particle.SDK/)

## Introduction

Particle Windows Cloud SDK enables Windows apps to interact with Particle-powered connected products via the Particle Cloud. It's an easy-to-use wrapper for Particle REST API. The Cloud SDK will allow you to:

- Manage user sessions for the Particle Cloud (access tokens, encrypted session management)
- Claim/Unclaim devices for a user account
- Get a list of instances of user's Particle devices
- Read variables from devices
- Invoke functions on devices
- Publish events and subscribe to events coming from devices

All cloud operations take place asynchronously and return a *System.Threading.Tasks.Task* allowing you to build beautiful responsive apps for your Particle products and projects. Windows Cloud SDK is implemented as an open-source .NET Standard Class Library. See [Installation](#installation) section for more details.

## Beta notice

This SDK is still under development and is currently released as Beta and over the next few months may go under considerable changes. Although tested, bugs and issues may be present. Some code might require cleanup. In addition, until version 1.0 is released, we cannot guarantee that API calls will not break from one Cloud SDK version to the next. Be sure to consult the [Change Log](https://github.com/spark/particle-windows-sdk/blob/master/CHANGELOG.md) for any breaking changes / additions to the SDK.

## Getting started

- Perform the installation step described under the [Installation](#installation) section below for integrating in your own project
- Be sure to check [Usage](#usage) before you begin for some code examples

## Usage

Cloud SDK usage involves two basic classes: first is [`ParticleCloud`](https://github.com/spark/particle-windows-sdk/blob/master/ParticleCloud.cs) which is an object that enables all basic cloud operations such as user authentication, device listing, claiming etc. Second class is [`ParticleDevice`](https://github.com/spark/particle-windows-sdk/blob/master/ParticleDevice.cs) which is an instance representing a claimed device in the current user session. Each object enables device-specific operation such as: getting its info, invoking functions and reading variables from it.

## SDK calls from the UI thread

Some calls from the SDK can both update properties or run callbacks on a non UI thread (e.g. Events). If your application has a UI thread make sure to set the `SynchronizationContext`.

```cs
ParticleCloud.SharedCloud.SynchronizationContext = System.Threading.SynchronizationContext.Current;
```

## Common tasks

Here are few examples for the most common use cases to get your started:

### Log in to Particle Cloud

You don't need to worry about access tokens and session expiry, SDK takes care of that for you.

```cs
var success = await ParticleCloud.SharedCloud.LoginAsync("user@example.com", "myl33tp4ssw0rd");
```

### Log in to Particle Cloud with a token and validate

```cs
var success = await ParticleCloud.SharedCloud.TokenLoginAsync("d4f69e3a357f78316d50e76dbf10fe92364154bf");
```

### Injecting an access token (app utilizes two legged authentication)

```cs
var success = await ParticleCloud.SharedCloud.SetAuthentication("d4f69e3a357f78316d50e76dbf10fe92364154bf");
```

### Get a list of all devices

List the devices that belong to currently logged in user and find a specific device by name:

```cs
ParticleDevice myDevice = null;
List<ParticleDevice> devices = ParticleCloud.SharedCloud.GetDevicesAsync();
foreach (ParticleDevice device in devices)
{
  if (device.Name().equals("myDeviceName"))
    myDevice = device;
}
```

### Get device instance by its ID or name

```cs
ParticleDevice device = ParticleCloud.SharedCloud.GetDeviceAsync("e9eb56e90e703f602d67ceb3");
```

### Read a variable from a Particle device

Assuming here that `myDevice` is an active instance of `ParticleDevice` class which represents a device claimed to current user.

```cs
var variableResponse = myDevice.GetVariableAsync("temperature");
int temperatureReading = (int)variableResponse.Result;
```

### Call a function on a Particle device

Invoke a function on the device and pass a parameter to it, the returning `ParticleFunctionResponse` will represent the returned result data of the function on the device.

```cs
int functionResponse = myDevice.RunFunctionAsync("digitalwrite", "D7 HIGH"));
int result = functionResponse.ReturnValue;
```

### List device exposed functions and variables

`ParticleDevice.Functions` returns a list of function names. `ParticleDevice.Variables` returns a dictionary of variable names to types.

```cs
foreach (string functionName in myDevice.Functions)
  Debug.WriteLine($"Device has function: {functionName}");

foreach (varvariable in myDevice.Variables)
  Debug.WriteLine($"Device has variable: '{variable.Key}' of type '{variable.Value}'");
```

### Rename a device

Set a new name for a claimed device:

```cs
myDevice.RenameAsync("myDeviceNew");
```

### Refresh a device

Refreshes all the locally stored properties from the cloud and physical device (if online):

```cs
myDevice.RefreshAsync();
```

### Signal a device

Send a signal to the device to shout rainbows:

```cs
myDevice.SignalAsync(true);
```

### Log out

Log out the user, clearing their session and access token:

```cs
ParticleCloud.SharedCloud.LogOut();
```

## Events sub-system
You can make an API call that will open a stream of [Server-Sent Events (SSEs)](http://www.w3.org/TR/eventsource/). You will make one API call that opens a connection to the Particle Cloud. That connection will stay open, unlike normal HTTP calls which end quickly. Very little data will come to you across the connection unless your Particle device publishes an event, at which point you will be immediately notified. In each case, the event name filter is `eventNamePrefix` and is optional. When specifying an event name filter, published events will be limited to those events with names that begin with the specified string. For example, specifying an event name filter of 'temp' will return events with names 'temp' and 'temperature'.

### Subscribe to events

Subscribe to the firehose of public events, plus private events published by devices one owns:

```cs
private void onEvent(object sender, ParticleEventResponse particeEvent)
{
  Debug.WriteLine($"Got Event {particeEvent.Name} with data {particeEvent.Data}");
}

Guid eventListenerID = ParticleCloud.SharedCloud.SubscribeToAllEventsWithPrefixAsync(onEvent, "temp");
```

*Note:* specifying null or empty string in the `eventNamePrefix` parameter will subscribe to ALL events (lots of data!) You can have multiple handlers per event name and/or same handler per multiple events names.

Subscribe to all events, public and private, published by devices the user owns:

```cs
Guid eventListenerID = ParticleCloud.SharedCloud.SubscribeToDevicesEventsWithPrefixAsync(handler, "temp");
```

Subscribe to events from one specific device. Pass a `PaticleDevice` or `deviceId` string as a second parameter. If the API user owns the device, then all events, public and private, published by that device will be received. If the API user does not own the device only public events will be received.

```cs
Guid eventListenerID = ParticleCloud.SharedCloud.SubscribeToDeviceEventsWithPrefixAsync(handler, myDevice);
```

```cs
Guid eventListenerID = ParticleCloud.SharedCloud.SubscribeToDeviceEventsWithPrefixAsync(handler, "e9eb56e90e703f602d67ceb3");
```

The method `SubscribeToDeviceEventsWithPrefixAsync` can also be called on a `ParticleDevice` instance, guaranteeing that private events will be received since having access device instance in your app signifies that the user has this device claimed.

```cs
Guid eventListenerID = myDevice.SubscribeToDeviceEventsWithPrefixAsync(handler, "temp");
```

### Unsubscribing from events

Very straightforward. Keep the id object the subscribe method returned and use it as parameter to call the unsubscribe method:

```cs
ParticleCloud.SharedCloud.UnsubscribeFromEvent(eventListenerID);
```

or via the `ParticleDevice` instance (if applicable):

```cs
myDevice.UnsubscribeFromEvent(eventListenerID);
```

### Publishing an event

You can also publish an event from your app to the Particle Cloud:

```cs
ParticleCloud.SharedCloud.PublishEventAsync("event_from_app", "event_payload", true, 60);
```

## OAuth client configuration

If you're creating an app you're required to provide the `ParticleCloud` class with OAuth clientId and secret. Those are used to identify users coming from your specific app to the Particle Cloud. Please follow the procedure described [in our guide](https://docs.particle.io/reference/api/#create-an-oauth-client) to create those strings.

Once you've created your OAuth credentials, you can supply them to the SDK by providing them as string resources in a string resource file called "OAuthClient.resw", using the names `OAuthClientID` and `OAuthClientSecret` and they'll be picked up by the SDK automatically:

```xml
<data name="OAuthClientID" xml:space="preserve">
  <value>(client ID string goes here)</value>
</data>
<data name="OAuthClientSecret" xml:space="preserve">
  <value>(client secret 40-char hex string goes here)</value>
</data>
```

If you aren't creating a Windows Store app and/or not using string resources you can manually set the values. Make sure you do this before calling any other functions.

```cs
ParticleCloud.SharedCloud.OAuthClientId  = "(client ID string goes here)";
ParticleCloud.SharedCloud.OAuthClientSecret  = "(client secret 40-char hex string goes here)";
```

## Installation
- .NET Standard 2.1 Framework for use in full .NetFramework, .Net Core, Mono, Xamarin, UWP (https://docs.microsoft.com/en-us/dotnet/standard/net-standard)
- Any edition of Microsoft Visual Studio 2017 or 2019 (Other build systems may also work, but are not officially supported.)
- You can use either C# or VB

You can either [download Particle Windows Cloud SDK](https://github.com/spark/particle-windows-sdk/archive/master.zip) or install using [NuGet](http://www.nuget.org/packages/Particle.SDK)

`PM> Install-Package Particle.SDK`

## Communication

- If you **need help**, use [Our community website](http://community.particle.io), use the `Mobile` category for discussion/troubleshooting Windows apps using the Particle Windows Cloud SDK.
- If you are certain you **found a bug**, _and can provide steps to reliably reproduce it_, [open an issue on GitHub](https://github.com/spark/particle-windows-sdk/labels/bug).
- If you **have a feature request**, [open an issue on GitHub](https://github.com/spark/particle-windows-sdk/labels/enhancement).
- If you **want to contribute**, submit a pull request, be sure to check out spark.github.io for our contribution guidelines, and please sign the [CLA](https://docs.google.com/a/particle.io/forms/d/1_2P-vRKGUFg5bmpcKLHO_qNZWGi5HKYnfrrkd-sbZoA/viewform).

## Maintainers

- Justin Myers [Github](https://github.com/justmobilize)
- Ido Kleinman [Github](https://www.github.com/idokleinman) | [Twitter](https://www.twitter.com/idokleinman)

## License

Particle Windows Cloud SDK is available under the Apache License 2.0. See the [LICENSE file](https://github.com/spark/particle-windows-sdk/blob/master/LICENSE) for more info.

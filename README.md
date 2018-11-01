# Syncromatics.Clients.WaySine

A .NET library to interact with the [WaySine](http://waysine.com/) WayManager API to query for sign
status updates.

## Quickstart

Add the `Syncromatics.Clients.WaySine` package to your project:

```bash
dotnet add package Syncromatics.Clients.WaySine
```

Then, create a `ClientSettings` object to specify your server's root URL and use it to initialize a
`WayManagerClient`.

```csharp
using Syncromatics.Client.WaySine;

var clientSettings = new ClientSettings(serverRootUrl: "http://example.com/");
IWayManagerClient client = new WayManagerClient(clientSettings);

// to get information about every sign available:
var allSigns = await client.GetAllSignsAsync();

// to get information about a particular sign:
const int signId = 42;
var onlyOneSign = await client.GetSignAsync(signId);
```

## Building

[![Travis](https://img.shields.io/travis/syncromatics/Syncromatics.Clients.WaySine.svg)](https://travis-ci.org/syncromatics/Syncromatics.Clients.WaySine)
[![NuGet](https://img.shields.io/nuget/v/Syncromatics.Clients.WaySine.svg)](https://www.nuget.org/packages/Syncromatics.Clients.WaySine/)
[![NuGet Pre Release](https://img.shields.io/nuget/vpre/Syncromatics.Clients.WaySine.svg)](https://www.nuget.org/packages/Syncromatics.Clients.WaySine/)

This library is built using Cake.

### Build and test on Windows

```
build.ps1 -Experimental
```

### Build and test on Linux

```
./build.sh
```

## Code of Conduct

We are committed to fostering an open and welcoming environment. Please read our [code of conduct](CODE_OF_CONDUCT.md) before participating in or contributing to this project.

## Contributing

We welcome contributions and collaboration on this project. Please read our [contributor's guide](CONTRIBUTING.md) to understand how best to work with us.

## License and Authors

[![GMV Syncromatics Engineering logo](https://secure.gravatar.com/avatar/645145afc5c0bc24ba24c3d86228ad39?size=16) GMV Syncromatics Engineering](https://github.com/syncromatics)

![license](https://img.shields.io/github/license/Syncromatics/Syncromatics.Clients.WaySine.svg)
![GitHub contributors](https://img.shields.io/github/contributors/Syncromatics/Syncromatics.Clients.WaySine.svg)

This software is made available by GMV Syncromatics Engineering under the MIT license.
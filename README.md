---
outputFileName: index.html
---
# Registry Client

[![Build status](https://ci.appveyor.com/api/projects/status/39wuvnkvpg0ahyya?svg=true)](https://ci.appveyor.com/project/shirhatti/registryclient)
![MyGet](https://img.shields.io/myget/shirhatti-registryclient/v/RegistryClient.svg)

Registry Client is a client library targetting .NET Standard 2.0 that provides an easy way to interact with the [Docker Registry HTTP API V2](https://docs.docker.com/registry/spec/api/)

## Usage example

Get list of tags for a repository

```c#
var repo = "microsoft/dotnet";
var dockerHubRegistry = new Registry();
var tags = await dockerHubRegistry.GetTagsAsync(repo);
foreach(var tag in tags)
{
    Console.WriteLine(tag);
}
```

## Additional Resources

- [Docs](https://cdn.rawgit.com/shirhatti/RegistryClient/gh-pages/)
- [MyGet](https://www.myget.org/feed/shirhatti-registryclient/package/nuget/RegistryClient)
- [AppVeyor](https://ci.appveyor.com/project/shirhatti/registryclient)